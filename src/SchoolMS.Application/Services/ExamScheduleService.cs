using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class ExamScheduleService : IExamScheduleService
{
    private readonly IRepository<ExamSchedule> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ExamScheduleService(IRepository<ExamSchedule> repository, IUnitOfWork unitOfWork, IMapper mapper)
    { _repository = repository; _unitOfWork = unitOfWork; _mapper = mapper; }

    public async Task<List<ExamScheduleDto>> GetAllAsync()
    {
        var items = await _repository.Query()
            .Include(e => e.ExamType).Include(e => e.ClassRoom).ThenInclude(c => c.Grade)
            .Include(e => e.ClassRoom).ThenInclude(c => c.Division)
            .Include(e => e.ClassRoom).ThenInclude(c => c.Branch)
            .Include(e => e.Subject).Include(e => e.Teacher).Include(e => e.AcademicYear)
            .Include(e => e.School)
            .ToListAsync();
        return _mapper.Map<List<ExamScheduleDto>>(items);
    }

    public async Task<List<ExamScheduleDto>> GetBySchoolIdAsync(int schoolId, int? branchId = null, int? examTypeId = null,
        int? classRoomId = null, int? subjectId = null, int? teacherId = null, int? academicYearId = null)
    {
        var query = _repository.Query().Where(e => e.SchoolId == schoolId);
        if (branchId.HasValue) query = query.Where(e => e.ClassRoom.BranchId == branchId.Value);
        if (examTypeId.HasValue) query = query.Where(e => e.ExamTypeId == examTypeId.Value);
        if (classRoomId.HasValue) query = query.Where(e => e.ClassRoomId == classRoomId.Value);
        if (subjectId.HasValue) query = query.Where(e => e.SubjectId == subjectId.Value);
        if (teacherId.HasValue) query = query.Where(e => e.TeacherId == teacherId.Value);
        if (academicYearId.HasValue) query = query.Where(e => e.AcademicYearId == academicYearId.Value);
        var items = await query
            .Include(e => e.ExamType).Include(e => e.ClassRoom).ThenInclude(c => c.Grade)
            .Include(e => e.ClassRoom).ThenInclude(c => c.Division)
            .Include(e => e.ClassRoom).ThenInclude(c => c.Branch)
            .Include(e => e.Subject).Include(e => e.Teacher).Include(e => e.AcademicYear)
            .Include(e => e.School)
            .OrderBy(e => e.ExamDate).ThenBy(e => e.StartTime)
            .ToListAsync();
        return _mapper.Map<List<ExamScheduleDto>>(items);
    }

    public async Task<List<ExamScheduleDto>> GetByClassRoomIdsAsync(List<int> classRoomIds, int schoolId)
    {
        if (classRoomIds.Count == 0) return new List<ExamScheduleDto>();
        var items = await _repository.Query()
            .Where(e => e.SchoolId == schoolId && classRoomIds.Contains(e.ClassRoomId))
            .Include(e => e.ExamType).Include(e => e.ClassRoom).ThenInclude(c => c.Grade)
            .Include(e => e.ClassRoom).ThenInclude(c => c.Division)
            .Include(e => e.ClassRoom).ThenInclude(c => c.Branch)
            .Include(e => e.Subject).Include(e => e.Teacher).Include(e => e.AcademicYear)
            .Include(e => e.School)
            .OrderBy(e => e.ExamDate).ThenBy(e => e.StartTime)
            .ToListAsync();
        return _mapper.Map<List<ExamScheduleDto>>(items);
    }

    public async Task<ExamScheduleDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.Query()
            .Include(e => e.ExamType).Include(e => e.ClassRoom).ThenInclude(c => c.Grade)
            .Include(e => e.ClassRoom).ThenInclude(c => c.Division)
            .Include(e => e.ClassRoom).ThenInclude(c => c.Branch)
            .Include(e => e.Subject).Include(e => e.Teacher).Include(e => e.AcademicYear)
            .Include(e => e.School)
            .FirstOrDefaultAsync(e => e.Id == id);
        return entity == null ? null : _mapper.Map<ExamScheduleDto>(entity);
    }

    public async Task<ExamScheduleDto> CreateAsync(ExamScheduleDto dto)
    {
        var entity = _mapper.Map<ExamSchedule>(dto); entity.Id = 0;
        await _repository.AddAsync(entity); await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<ExamScheduleDto>(entity);
    }

    public async Task CreateBulkAsync(List<ExamScheduleDto> dtos)
    {
        foreach (var dto in dtos)
        {
            var entity = _mapper.Map<ExamSchedule>(dto);
            entity.Id = 0;
            await _repository.AddAsync(entity);
        }
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<ExamScheduleDto> UpdateAsync(ExamScheduleDto dto)
    {
        var entity = await _repository.GetByIdAsync(dto.Id) ?? throw new KeyNotFoundException();
        entity.ExamTypeId = dto.ExamTypeId; entity.ClassRoomId = dto.ClassRoomId;
        entity.SubjectId = dto.SubjectId; entity.TeacherId = dto.TeacherId;
        entity.ExamDate = dto.ExamDate; entity.StartTime = dto.StartTime; entity.EndTime = dto.EndTime;
        entity.AcademicYearId = dto.AcademicYearId;
        if (dto.SchoolId > 0) entity.SchoolId = dto.SchoolId;
        _repository.Update(entity); await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<ExamScheduleDto>(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id) ?? throw new KeyNotFoundException();
        entity.IsDeleted = true; entity.DeletedAt = DateTime.UtcNow;
        _repository.Update(entity); await _unitOfWork.SaveChangesAsync();
    }

    public async Task<byte[]> ExportToExcelAsync()
    {
        var items = await GetAllAsync();
        using var workbook = new ClosedXML.Excel.XLWorkbook();
        workbook.Worksheets.Add("ExamSchedule");
        var ws = workbook.Worksheet("ExamSchedule");
        ws.Cell(1, 1).Value = "Exam Type"; ws.Cell(1, 2).Value = "Grade"; ws.Cell(1, 3).Value = "Division";
        ws.Cell(1, 4).Value = "Subject"; ws.Cell(1, 5).Value = "Teacher"; ws.Cell(1, 6).Value = "Date";
        ws.Cell(1, 7).Value = "Start"; ws.Cell(1, 8).Value = "End";
        ws.Range("A1:H1").Style.Font.Bold = true;
        for (int i = 0; i < items.Count; i++)
        {
            ws.Cell(i + 2, 1).Value = items[i].ExamTypeName;
            ws.Cell(i + 2, 2).Value = items[i].GradeName;
            ws.Cell(i + 2, 3).Value = items[i].DivisionName;
            ws.Cell(i + 2, 4).Value = items[i].SubjectName;
            ws.Cell(i + 2, 5).Value = items[i].TeacherName;
            ws.Cell(i + 2, 6).Value = items[i].ExamDate.ToString("yyyy-MM-dd");
            ws.Cell(i + 2, 7).Value = items[i].StartTime.ToString(@"hh\:mm");
            ws.Cell(i + 2, 8).Value = items[i].EndTime.ToString(@"hh\:mm");
        }
        ws.Columns().AdjustToContents();
        using var stream = new MemoryStream();
        workbook.SaveAs(stream); return stream.ToArray();
    }
}

