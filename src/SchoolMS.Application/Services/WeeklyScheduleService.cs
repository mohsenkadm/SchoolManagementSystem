using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class WeeklyScheduleService : IWeeklyScheduleService
{
    private readonly IRepository<WeeklySchedule> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public WeeklyScheduleService(IRepository<WeeklySchedule> repository, IUnitOfWork unitOfWork, IMapper mapper)
    { _repository = repository; _unitOfWork = unitOfWork; _mapper = mapper; }

    public async Task<List<WeeklyScheduleDto>> GetAllAsync()
    {
        var items = await _repository.Query()
            .Include(w => w.ClassRoom).ThenInclude(c => c.Grade)
            .Include(w => w.ClassRoom).ThenInclude(c => c.Division)
            .Include(w => w.ClassRoom).ThenInclude(c => c.Branch)
            .Include(w => w.Subject).Include(w => w.Teacher).Include(w => w.AcademicYear)
            .Include(w => w.School)
            .ToListAsync();
        return _mapper.Map<List<WeeklyScheduleDto>>(items);
    }

    public async Task<List<WeeklyScheduleDto>> GetBySchoolIdAsync(int schoolId)
    {
        var items = await _repository.Query()
            .Where(w => w.SchoolId == schoolId)
            .Include(w => w.ClassRoom).ThenInclude(c => c.Grade)
            .Include(w => w.ClassRoom).ThenInclude(c => c.Division)
            .Include(w => w.ClassRoom).ThenInclude(c => c.Branch)
            .Include(w => w.Subject).Include(w => w.Teacher).Include(w => w.AcademicYear)
            .Include(w => w.School)
            .ToListAsync();
        return _mapper.Map<List<WeeklyScheduleDto>>(items);
    }

    public async Task<List<WeeklyScheduleDto>> GetByClassRoomIdsAsync(List<int> classRoomIds, int schoolId)
    {
        if (classRoomIds.Count == 0) return new List<WeeklyScheduleDto>();
        var items = await _repository.Query()
            .Where(w => w.SchoolId == schoolId && classRoomIds.Contains(w.ClassRoomId))
            .Include(w => w.ClassRoom).ThenInclude(c => c.Grade)
            .Include(w => w.ClassRoom).ThenInclude(c => c.Division)
            .Include(w => w.ClassRoom).ThenInclude(c => c.Branch)
            .Include(w => w.Subject).Include(w => w.Teacher).Include(w => w.AcademicYear)
            .Include(w => w.School)
            .ToListAsync();
        return _mapper.Map<List<WeeklyScheduleDto>>(items);
    }

    public async Task<WeeklyScheduleDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.Query()
            .Include(w => w.ClassRoom).ThenInclude(c => c.Grade)
            .Include(w => w.ClassRoom).ThenInclude(c => c.Division)
            .Include(w => w.ClassRoom).ThenInclude(c => c.Branch)
            .Include(w => w.Subject).Include(w => w.Teacher).Include(w => w.AcademicYear)
            .Include(w => w.School)
            .FirstOrDefaultAsync(w => w.Id == id);
        return entity == null ? null : _mapper.Map<WeeklyScheduleDto>(entity);
    }

    public async Task<WeeklyScheduleDto> CreateAsync(WeeklyScheduleDto dto)
    {
        var entity = _mapper.Map<WeeklySchedule>(dto); entity.Id = 0;
        await _repository.AddAsync(entity); await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<WeeklyScheduleDto>(entity);
    }

    public async Task CreateBulkAsync(List<WeeklyScheduleDto> dtos)
    {
        foreach (var dto in dtos)
        {
            var entity = _mapper.Map<WeeklySchedule>(dto);
            entity.Id = 0;
            await _repository.AddAsync(entity);
        }
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<WeeklyScheduleDto> UpdateAsync(WeeklyScheduleDto dto)
    {
        var entity = await _repository.GetByIdAsync(dto.Id) ?? throw new KeyNotFoundException();
        entity.ClassRoomId = dto.ClassRoomId; entity.SubjectId = dto.SubjectId;
        entity.TeacherId = dto.TeacherId; entity.DayOfWeek = dto.DayOfWeek;
        entity.StartTime = dto.StartTime; entity.EndTime = dto.EndTime;
        entity.AcademicYearId = dto.AcademicYearId;
        if (dto.SchoolId > 0) entity.SchoolId = dto.SchoolId;
        _repository.Update(entity); await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<WeeklyScheduleDto>(entity);
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
        workbook.Worksheets.Add("WeeklySchedule");
        var ws = workbook.Worksheet("WeeklySchedule");
        ws.Cell(1, 1).Value = "Day"; ws.Cell(1, 2).Value = "Grade"; ws.Cell(1, 3).Value = "Division";
        ws.Cell(1, 4).Value = "Subject"; ws.Cell(1, 5).Value = "Teacher";
        ws.Cell(1, 6).Value = "Start"; ws.Cell(1, 7).Value = "End";
        ws.Range("A1:G1").Style.Font.Bold = true;
        for (int i = 0; i < items.Count; i++)
        {
            ws.Cell(i + 2, 1).Value = items[i].DayOfWeek.ToString();
            ws.Cell(i + 2, 2).Value = items[i].GradeName;
            ws.Cell(i + 2, 3).Value = items[i].DivisionName;
            ws.Cell(i + 2, 4).Value = items[i].SubjectName;
            ws.Cell(i + 2, 5).Value = items[i].TeacherName;
            ws.Cell(i + 2, 6).Value = items[i].StartTime.ToString(@"hh\:mm");
            ws.Cell(i + 2, 7).Value = items[i].EndTime.ToString(@"hh\:mm");
        }
        ws.Columns().AdjustToContents();
        using var stream = new MemoryStream();
        workbook.SaveAs(stream); return stream.ToArray();
    }
}

