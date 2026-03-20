using SchoolMS.Application.DTOs;

namespace SchoolMS.Application.Interfaces;

public interface IOtpService
{
    Task<SendOtpResponseDto> SendOtpAsync(SendOtpRequestDto request);
    Task<VerifyOtpResponseDto> VerifyOtpAsync(VerifyOtpRequestDto request);
    Task<bool> ValidateVerificationTokenAsync(string phone, string token);
}
