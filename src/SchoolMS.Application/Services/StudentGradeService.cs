using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class StudentGradeService : IStudentGradeService
{
    private readonly IRepository<StudentGrade> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public StudentGradeService(IRepository<StudentGrade> repository, IUnitOfWork unitOfWork, IMapper mapper)
    { _repository = repository; _unitOfWork = unitOfWork; _mapper = mapper; }

    public async Task<List<StudentGradeDto>> GetAllAsync()
    {
        var items = await _repository.Query()
            .Include(g => g.Student).Include(g => g.Subject)
            .Include(g => g.ExamType).Include(g => g.AcademicYear)
            .Include(g => g.School)
            .ToListAsync();
        return _mapper.Map<List<StudentGradeDto>>(items);
    }

    public async Task<List<StudentGradeDto>> GetBySchoolIdAsync(int schoolId,
        int? subjectId = null, int? examTypeId = null, int? academicYearId = null)
    {
        var query = _repository.Query().Where(g => g.SchoolId == schoolId);
        if (subjectId.HasValue) query = query.Where(g => g.SubjectId == subjectId.Value);
        if (examTypeId.HasValue) query = query.Where(g => g.ExamTypeId == examTypeId.Value);
        if (academicYearId.HasValue) query = query.Where(g => g.AcademicYearId == academicYearId.Value);
        var items = await query
            .Include(g => g.Student).Include(g => g.Subject)
            .Include(g => g.ExamType).Include(g => g.AcademicYear)
            .Include(g => g.School)
            .ToListAsync();
        return _mapper.Map<List<StudentGradeDto>>(items);
    }

    public async Task<List<StudentGradeDto>> GetByClassRoomIdsAsync(List<int> classRoomIds, int schoolId,
        int? subjectId = null, int? examTypeId = null, int? academicYearId = null)
    {
        if (classRoomIds.Count == 0) return new List<StudentGradeDto>();
        var query = _repository.Query()
            .Where(g => g.SchoolId == schoolId)
            .Include(g => g.Student)
            .Where(g => classRoomIds.Contains(g.Student.ClassRoomId));
        if (subjectId.HasValue) query = query.Where(g => g.SubjectId == subjectId.Value);
        if (examTypeId.HasValue) query = query.Where(g => g.ExamTypeId == examTypeId.Value);
        if (academicYearId.HasValue) query = query.Where(g => g.AcademicYearId == academicYearId.Value);
        var gradeIds = await query.Select(g => g.Id).ToListAsync();
        var items = await _repository.Query()
            .Where(g => gradeIds.Contains(g.Id))
            .Include(g => g.Student).Include(g => g.Subject)
            .Include(g => g.ExamType).Include(g => g.AcademicYear)
            .Include(g => g.School)
            .ToListAsync();
        return _mapper.Map<List<StudentGradeDto>>(items);
    }

    public async Task<StudentGradeDto?> GetByIdAsync(int id)
    {
        var entity = await _repository.Query()
            .Include(g => g.Student).Include(g => g.Subject).Include(g => g.ExamType)
            .FirstOrDefaultAsync(g => g.Id == id);
        return entity == null ? null : _mapper.Map<StudentGradeDto>(entity);
    }

    public async Task<StudentGradeDto> CreateAsync(StudentGradeDto dto)
    {
        var entity = _mapper.Map<StudentGrade>(dto); entity.Id = 0;
        entity.GradeLetter = CalculateGradeLetter(dto.Mark, dto.MaxMark);
        await _repository.AddAsync(entity); await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<StudentGradeDto>(entity);
    }

    public async Task<StudentGradeDto> UpdateAsync(StudentGradeDto dto)
    {
        var entity = await _repository.GetByIdAsync(dto.Id) ?? throw new KeyNotFoundException();
        entity.StudentId = dto.StudentId; entity.SubjectId = dto.SubjectId;
        entity.ExamTypeId = dto.ExamTypeId; entity.Mark = dto.Mark; entity.MaxMark = dto.MaxMark;
        entity.GradeLetter = CalculateGradeLetter(dto.Mark, dto.MaxMark);
        entity.AcademicYearId = dto.AcademicYearId; entity.SchoolId = dto.SchoolId;
        _repository.Update(entity); await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<StudentGradeDto>(entity);
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id) ?? throw new KeyNotFoundException();
        entity.IsDeleted = true; entity.DeletedAt = DateTime.UtcNow;
        _repository.Update(entity); await _unitOfWork.SaveChangesAsync();
    }

    public async Task BulkCreateAsync(List<StudentGradeDto> dtos)
    {
        foreach (var dto in dtos)
        {
            var entity = _mapper.Map<StudentGrade>(dto); entity.Id = 0;
            entity.GradeLetter = CalculateGradeLetter(dto.Mark, dto.MaxMark);
            await _repository.AddAsync(entity);
        }
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<byte[]> ExportToExcelAsync()
    {
        var items = await GetAllAsync();
        using var workbook = new ClosedXML.Excel.XLWorkbook();
        workbook.Worksheets.Add("Grades");
        var ws = workbook.Worksheet("Grades");
        ws.Cell(1, 1).Value = "Student"; ws.Cell(1, 2).Value = "Subject"; ws.Cell(1, 3).Value = "Exam Type";
        ws.Cell(1, 4).Value = "Mark"; ws.Cell(1, 5).Value = "Max Mark"; ws.Cell(1, 6).Value = "Grade";
        ws.Range("A1:F1").Style.Font.Bold = true;
        for (int i = 0; i < items.Count; i++)
        {
            ws.Cell(i + 2, 1).Value = items[i].StudentName;
            ws.Cell(i + 2, 2).Value = items[i].SubjectName;
            ws.Cell(i + 2, 3).Value = items[i].ExamTypeName;
            ws.Cell(i + 2, 4).Value = items[i].Mark;
            ws.Cell(i + 2, 5).Value = items[i].MaxMark;
            ws.Cell(i + 2, 6).Value = items[i].GradeLetter;
        }
        ws.Columns().AdjustToContents();
        using var stream = new MemoryStream();
        workbook.SaveAs(stream); return stream.ToArray();
    }

    private static string CalculateGradeLetter(decimal mark, decimal maxMark)
    {
        if (maxMark == 0) return "N/A";
        var percentage = mark / maxMark * 100;
        return percentage switch
        {
            >= 90 => "A",
            >= 80 => "B",
            >= 70 => "C",
            >= 60 => "D",
            _ => "F"
        };
    }
}

