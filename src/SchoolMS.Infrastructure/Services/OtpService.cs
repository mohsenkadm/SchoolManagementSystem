using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Infrastructure.Data;

namespace SchoolMS.Infrastructure.Services;

public class OtpService : IOtpService
{
    private readonly SchoolDbContext _context;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<OtpService> _logger;

    public OtpService(
        SchoolDbContext context,
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration,
        ILogger<OtpService> logger)
    {
        _context = context;
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<SendOtpResponseDto> SendOtpAsync(SendOtpRequestDto request)
    {
        var settings = _configuration.GetSection("OtpSettings");
        var maxAttempts = settings.GetValue("MaxSendAttempts", 3);
        var expiryMinutes = settings.GetValue("ExpiryMinutes", 5);

        // Check if phone is blocked
        var existing = await _context.OtpVerifications
            .Where(o => o.Phone == request.Phone && !o.IsUsed)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync();

        if (existing is { IsBlocked: true })
        {
            return new SendOtpResponseDto
            {
                Success = false,
                Message = "تم حظر هذا الرقم لتجاوز عدد المحاولات المسموح بها. تواصل مع الإدارة."
            };
        }

        // Count recent sends (last hour)
        var recentCount = await _context.OtpVerifications
            .CountAsync(o => o.Phone == request.Phone
                && o.CreatedAt > DateTime.UtcNow.AddHours(-1));

        if (recentCount >= maxAttempts)
        {
            // Block the phone
            if (existing != null)
            {
                existing.IsBlocked = true;
                _context.OtpVerifications.Update(existing);
                await _context.SaveChangesAsync();
            }

            return new SendOtpResponseDto
            {
                Success = false,
                Message = $"تم حظر الرقم لإرسالك أكثر من {maxAttempts} مرات. تواصل مع الإدارة."
            };
        }

        // Generate OTP code
        var otpCode = GenerateOtpCode();

        // Send OTP via WhatsApp (VerifyWay API)
        var sent = await SendOtpViaWhatsAppAsync(request.Phone, otpCode);
        if (!sent)
        {
            return new SendOtpResponseDto
            {
                Success = false,
                Message = "لم يتم إرسال كود التحقق. حاول مرة أخرى."
            };
        }

        // Save OTP record
        var otpRecord = new OtpVerification
        {
            Phone = request.Phone,
            Code = otpCode,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes),
            IsUsed = false,
            AttemptCount = 0,
            IsBlocked = false,
            Purpose = request.Purpose ?? "StudentRegistration"
        };

        _context.OtpVerifications.Add(otpRecord);
        await _context.SaveChangesAsync();

        _logger.LogInformation("OTP sent to {Phone} for {Purpose}", request.Phone, request.Purpose);

        return new SendOtpResponseDto
        {
            Success = true,
            Message = "تم إرسال كود التحقق بنجاح."
        };
    }

    public async Task<VerifyOtpResponseDto> VerifyOtpAsync(VerifyOtpRequestDto request)
    {
        var settings = _configuration.GetSection("OtpSettings");
        var maxVerifyAttempts = settings.GetValue("MaxVerifyAttempts", 5);

        var otpRecord = await _context.OtpVerifications
            .Where(o => o.Phone == request.Phone && !o.IsUsed && !o.IsBlocked)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync();

        if (otpRecord == null)
        {
            return new VerifyOtpResponseDto
            {
                Success = false,
                Message = "لا يوجد كود تحقق لهذا الرقم. أرسل كود جديد."
            };
        }

        if (otpRecord.ExpiresAt < DateTime.UtcNow)
        {
            return new VerifyOtpResponseDto
            {
                Success = false,
                Message = "انتهت صلاحية كود التحقق. أرسل كود جديد."
            };
        }

        otpRecord.AttemptCount++;

        if (otpRecord.AttemptCount > maxVerifyAttempts)
        {
            otpRecord.IsBlocked = true;
            _context.OtpVerifications.Update(otpRecord);
            await _context.SaveChangesAsync();

            return new VerifyOtpResponseDto
            {
                Success = false,
                Message = "تم حظر الرقم لتجاوز عدد محاولات التحقق."
            };
        }

        if (otpRecord.Code != request.Code)
        {
            _context.OtpVerifications.Update(otpRecord);
            await _context.SaveChangesAsync();

            return new VerifyOtpResponseDto
            {
                Success = false,
                Message = $"كود التحقق غير صحيح. المحاولات المتبقية: {maxVerifyAttempts - otpRecord.AttemptCount}"
            };
        }

        // Mark as used
        otpRecord.IsUsed = true;
        _context.OtpVerifications.Update(otpRecord);
        await _context.SaveChangesAsync();

        // Generate verification token (valid for 15 minutes)
        var token = GenerateVerificationToken(request.Phone);

        _logger.LogInformation("OTP verified for {Phone}", request.Phone);

        return new VerifyOtpResponseDto
        {
            Success = true,
            Message = "تم التحقق بنجاح.",
            VerificationToken = token
        };
    }

    public async Task<bool> ValidateVerificationTokenAsync(string phone, string token)
    {
        // Check if OTP was verified for this phone within last 15 minutes
        var verified = await _context.OtpVerifications
            .AnyAsync(o => o.Phone == phone
                && o.IsUsed
                && o.CreatedAt > DateTime.UtcNow.AddMinutes(-15));

        if (!verified) return false;

        // Validate token
        var expected = GenerateVerificationToken(phone);
        return token == expected;
    }

    private async Task<bool> SendOtpViaWhatsAppAsync(string phone, string otpCode)
    {
        try
        {
            var settings = _configuration.GetSection("OtpSettings");
            var apiKey = settings["ApiKey"]!;
            var baseUrl = settings["BaseUrl"] ?? "https://api.verifyway.com";
            var countryCode = settings["CountryCode"] ?? "964";
            var channel = settings["Channel"] ?? "whatsapp";

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            // Format phone: remove leading 0 and prepend country code
            var formattedPhone = phone.StartsWith("0")
                ? $"{countryCode}{phone[1..]}"
                : $"{countryCode}{phone}";

            var requestData = new
            {
                recipient = formattedPhone,
                type = "otp",
                code = otpCode,
                channel
            };

            var jsonContent = JsonSerializer.Serialize(requestData);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await client.PostAsync($"{baseUrl}/api/v1/", content);

            if (response.IsSuccessStatusCode)
            {
                var responseBody = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<OtpApiResponse>(responseBody,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

                return result?.Status == "success";
            }

            _logger.LogWarning("OTP send failed: {Status}", response.StatusCode);
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending OTP to {Phone}", phone);
            return false;
        }
    }

    private static string GenerateOtpCode()
    {
        return RandomNumberGenerator.GetInt32(100000, 999999).ToString();
    }

    private string GenerateVerificationToken(string phone)
    {
        var secret = _configuration["OtpSettings:TokenSecret"] ?? "SchoolMS-OTP-Token-Secret-Key";
        var today = DateTime.UtcNow.ToString("yyyyMMddHH"); // Valid per hour block
        var raw = $"{phone}:{today}:{secret}";
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(raw));
        return Convert.ToBase64String(bytes);
    }

    private class OtpApiResponse
    {
        public string Status { get; set; } = string.Empty;
        public string? Message { get; set; }
    }
}
