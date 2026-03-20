using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class PromotionService : IPromotionService
{
    private readonly IRepository<Student> _studentRepo;
    private readonly IRepository<StudentPromotion> _promotionRepo;
    private readonly IRepository<ClassRoom> _classRoomRepo;
    private readonly IUnitOfWork _unitOfWork;

    public PromotionService(IRepository<Student> studentRepo, IRepository<StudentPromotion> promotionRepo,
        IRepository<ClassRoom> classRoomRepo, IUnitOfWork unitOfWork)
    {
        _studentRepo = studentRepo; _promotionRepo = promotionRepo;
        _classRoomRepo = classRoomRepo; _unitOfWork = unitOfWork;
    }

    public async Task<List<StudentPromotionDto>> PreviewPromotionAsync(int fromClassRoomId, int toClassRoomId, int toAcademicYearId)
    {
        var students = await _studentRepo.Query()
            .Include(s => s.ClassRoom).ThenInclude(c => c.Grade)
            .Include(s => s.ClassRoom).ThenInclude(c => c.Division)
            .Where(s => s.ClassRoomId == fromClassRoomId)
            .ToListAsync();

        var toClassRoom = await _classRoomRepo.Query()
            .Include(c => c.Grade).Include(c => c.Division)
            .FirstOrDefaultAsync(c => c.Id == toClassRoomId);

        return students.Select(s => new StudentPromotionDto
        {
            StudentId = s.Id,
            StudentName = s.FullName,
            FullName = s.FullName,
            BadgeCardNumber = s.BadgeCardNumber,
            GradeName = s.ClassRoom?.Grade?.GradeName,
            DivisionName = s.ClassRoom?.Division?.DivisionName,
            FromClassRoomId = s.ClassRoomId,
            FromClassName = $"{s.ClassRoom?.Grade?.GradeName} - {s.ClassRoom?.Division?.DivisionName}",
            ToClassRoomId = toClassRoomId,
            ToClassName = toClassRoom != null ? $"{toClassRoom.Grade?.GradeName} - {toClassRoom.Division?.DivisionName}" : "",
            FromAcademicYearId = s.AcademicYearId,
            ToAcademicYearId = toAcademicYearId,
            Status = PromotionStatus.Promoted
        }).ToList();
    }

    public async Task ExecutePromotionAsync(List<StudentPromotionDto> promotions)
    {
        foreach (var p in promotions)
        {
            var student = await _studentRepo.GetByIdAsync(p.StudentId);
            if (student == null) continue;

            var record = new StudentPromotion
            {
                StudentId = p.StudentId,
                FromClassRoomId = p.FromClassRoomId,
                ToClassRoomId = p.ToClassRoomId,
                FromAcademicYearId = p.FromAcademicYearId,
                ToAcademicYearId = p.ToAcademicYearId,
                Status = p.Status,
                SchoolId = student.SchoolId
            };
            await _promotionRepo.AddAsync(record);

            if (p.Status == PromotionStatus.Promoted)
            {
                student.ClassRoomId = p.ToClassRoomId;
                student.AcademicYearId = p.ToAcademicYearId;
                _studentRepo.Update(student);
            }
        }
        await _unitOfWork.SaveChangesAsync();
    }
}

