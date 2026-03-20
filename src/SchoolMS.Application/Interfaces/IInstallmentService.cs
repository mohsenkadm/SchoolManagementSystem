using SchoolMS.Application.DTOs;
using SchoolMS.Domain.Enums;

namespace SchoolMS.Application.Interfaces;

public interface IInstallmentService
{
    Task<List<FeeInstallmentDto>> GetAllAsync();
    Task<List<FeeInstallmentDto>> GetBySchoolIdAsync(int schoolId, int? branchId = null);
    Task<List<FeeInstallmentDto>> GetByStudentIdAsync(int studentId, int schoolId);
    Task<List<FeeInstallmentDto>> GetByParentChildrenAsync(int parentId, int schoolId);
    Task<FeeInstallmentDto?> GetByIdAsync(int id);
    Task<FeeInstallmentDto> CreateAsync(FeeInstallmentDto dto);
    Task<FeeInstallmentDto> UpdateAsync(FeeInstallmentDto dto);
    Task DeleteAsync(int id);
    Task RecordPaymentAsync(int paymentId);
    Task CancelPaymentAsync(int paymentId);
    Task<List<InstallmentPaymentDto>> GetAllPaymentsAsync(PaymentStatus? statusFilter = null);
    Task<List<InstallmentPaymentDto>> GetPaymentsBySchoolIdAsync(int schoolId, PaymentStatus? statusFilter = null);
    Task<List<InstallmentPaymentDto>> GetOverdueAsync();
    Task<byte[]> ExportToExcelAsync();
    Task<byte[]> ExportPaymentsToExcelAsync(PaymentStatus? statusFilter = null);
    Task<byte[]> ExportStudentSummaryToExcelAsync();
}

