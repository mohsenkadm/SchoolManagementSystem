using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class TeacherService : ITeacherService
{
    private readonly IRepository<Teacher> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public TeacherService(IRepository<Teacher> repository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<TeacherDto>> GetAllAsync()
    {
        var teachers = await _repository.Query()
            .Include(t => t.Branch)
            .ToListAsync();
        return _mapper.Map<List<TeacherDto>>(teachers);
    }

    public async Task<List<TeacherDto>> GetBySchoolIdAsync(int schoolId)
    {
        var teachers = await _repository.Query()
            .Where(t => t.SchoolId == schoolId)
            .Include(t => t.Branch)
            .ToListAsync();
        return _mapper.Map<List<TeacherDto>>(teachers);
    }

    public async Task<TeacherDto?> GetByIdAsync(int id)
    {
        var teacher = await _repository.Query()
            .Include(t => t.Branch)
            .FirstOrDefaultAsync(t => t.Id == id);
        return teacher == null ? null : _mapper.Map<TeacherDto>(teacher);
    }

    public async Task<DataTableResponse<TeacherDto>> GetDataTableAsync(DataTableRequest request)
    {
        var query = _repository.Query().Include(t => t.Branch).Include(t => t.School).AsQueryable();

        if (request.SchoolId.HasValue)
            query = query.Where(t => t.SchoolId == request.SchoolId);
        if (request.BranchId.HasValue)
            query = query.Where(t => t.BranchId == request.BranchId);

        var totalRecords = await _repository.Query().CountAsync();

        if (!string.IsNullOrEmpty(request.SearchValue))
        {
            var search = request.SearchValue.ToLower();
            query = query.Where(t =>
                t.FullName.ToLower().Contains(search) ||
                (t.Phone != null && t.Phone.Contains(search)) ||
                (t.Specialization != null && t.Specialization.ToLower().Contains(search)));
        }

        var filteredRecords = await query.CountAsync();

        query = request.SortColumn?.ToLower() switch
        {
            "fullname" => request.SortDirection == "desc" ? query.OrderByDescending(t => t.FullName) : query.OrderBy(t => t.FullName),
            _ => query.OrderByDescending(t => t.Id)
        };

        var data = await query.Skip(request.Start).Take(request.Length).ToListAsync();

        return new DataTableResponse<TeacherDto>
        {
            Draw = request.Draw,
            RecordsTotal = totalRecords,
            RecordsFiltered = filteredRecords,
            Data = _mapper.Map<List<TeacherDto>>(data)
        };
    }

    public async Task<TeacherDto> CreateAsync(CreateTeacherDto dto)
    {
        var teacher = _mapper.Map<Teacher>(dto);
        await _repository.AddAsync(teacher);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<TeacherDto>(teacher);
    }

    public async Task<TeacherDto> UpdateAsync(UpdateTeacherDto dto)
    {
        var teacher = await _repository.GetByIdAsync(dto.Id)
            ?? throw new KeyNotFoundException($"Teacher with ID {dto.Id} not found.");
        _mapper.Map(dto, teacher);
        _repository.Update(teacher);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<TeacherDto>(teacher);
    }

    public async Task DeleteAsync(int id)
    {
        var teacher = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Teacher with ID {id} not found.");
        teacher.IsDeleted = true;
        teacher.DeletedAt = DateTime.UtcNow;
        _repository.Update(teacher);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<byte[]> ExportToExcelAsync()
    {
        var teachers = await GetAllAsync();
        using var workbook = new ClosedXML.Excel.XLWorkbook();
        workbook.Worksheets.Add("Teachers");
        var ws = workbook.Worksheet("Teachers");
        ws.Cell(1, 1).Value = "Name";
        ws.Cell(1, 2).Value = "Specialization";
        ws.Cell(1, 3).Value = "Phone";
        ws.Cell(1, 4).Value = "Email";
        ws.Cell(1, 5).Value = "Badge Card";
        ws.Cell(1, 6).Value = "Base Salary";
        ws.Cell(1, 7).Value = "Branch";

        ws.Range("A1:G1").Style.Font.Bold = true;
        ws.Range("A1:G1").Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.FromHtml("#16213e");
        ws.Range("A1:G1").Style.Font.FontColor = ClosedXML.Excel.XLColor.White;

        for (int i = 0; i < teachers.Count; i++)
        {
            ws.Cell(i + 2, 1).Value = teachers[i].FullName;
            ws.Cell(i + 2, 2).Value = teachers[i].Specialization;
            ws.Cell(i + 2, 3).Value = teachers[i].Phone;
            ws.Cell(i + 2, 4).Value = teachers[i].Email;
            ws.Cell(i + 2, 5).Value = teachers[i].BadgeCardNumber;
            ws.Cell(i + 2, 6).Value = teachers[i].BaseSalary;
            ws.Cell(i + 2, 7).Value = teachers[i].BranchName;
        }
        ws.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
