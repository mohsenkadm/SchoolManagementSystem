using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class StudentService : IStudentService
{
    private readonly IRepository<Student> _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public StudentService(IRepository<Student> repository, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<List<StudentDto>> GetAllAsync()
    {
        return await _repository.Query()
            .Include(s => s.Branch)
            .Include(s => s.ClassRoom).ThenInclude(c => c.Grade)
            .Include(s => s.ClassRoom).ThenInclude(c => c.Division)
            .Include(s => s.AcademicYear)
            .Select(s => _mapper.Map<StudentDto>(s))
            .ToListAsync();
    }

    public async Task<StudentDto?> GetByIdAsync(int id)
    {
        var student = await _repository.Query()
            .Include(s => s.Branch)
            .Include(s => s.ClassRoom).ThenInclude(c => c.Grade)
            .Include(s => s.ClassRoom).ThenInclude(c => c.Division)
            .Include(s => s.AcademicYear)
            .FirstOrDefaultAsync(s => s.Id == id);
        return student == null ? null : _mapper.Map<StudentDto>(student);
    }

    public async Task<DataTableResponse<StudentDto>> GetDataTableAsync(DataTableRequest request)
    {
        var query = _repository.Query()
            .Include(s => s.Branch)
            .Include(s => s.ClassRoom).ThenInclude(c => c.Grade)
            .Include(s => s.ClassRoom).ThenInclude(c => c.Division)
            .Include(s => s.AcademicYear)
            .Include(s => s.School)
            .AsQueryable();

        if (request.SchoolId.HasValue)
            query = query.Where(s => s.SchoolId == request.SchoolId);
        if (request.BranchId.HasValue)
            query = query.Where(s => s.BranchId == request.BranchId);
        if (request.AcademicYearId.HasValue)
            query = query.Where(s => s.AcademicYearId == request.AcademicYearId);
        if (request.ClassRoomId.HasValue)
            query = query.Where(s => s.ClassRoomId == request.ClassRoomId);

        var totalRecords = await _repository.Query().CountAsync();

        if (!string.IsNullOrEmpty(request.SearchValue))
        {
            var search = request.SearchValue.ToLower();
            query = query.Where(s =>
                s.FullName.ToLower().Contains(search) ||
                (s.Phone != null && s.Phone.Contains(search)) ||
                (s.BadgeCardNumber != null && s.BadgeCardNumber.Contains(search)) ||
                (s.ParentPhone != null && s.ParentPhone.Contains(search)));
        }

        var filteredRecords = await query.CountAsync();

        query = request.SortColumn?.ToLower() switch
        {
            "fullname" => request.SortDirection == "desc" ? query.OrderByDescending(s => s.FullName) : query.OrderBy(s => s.FullName),
            "phone" => request.SortDirection == "desc" ? query.OrderByDescending(s => s.Phone) : query.OrderBy(s => s.Phone),
            _ => query.OrderByDescending(s => s.Id)
        };

        var data = await query.Skip(request.Start).Take(request.Length).ToListAsync();

        return new DataTableResponse<StudentDto>
        {
            Draw = request.Draw,
            RecordsTotal = totalRecords,
            RecordsFiltered = filteredRecords,
            Data = _mapper.Map<List<StudentDto>>(data)
        };
    }

    public async Task<StudentDto> CreateAsync(CreateStudentDto dto)
    {
        var student = _mapper.Map<Student>(dto);
        await _repository.AddAsync(student);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<StudentDto>(student);
    }

    public async Task<StudentDto> UpdateAsync(UpdateStudentDto dto)
    {
        var student = await _repository.GetByIdAsync(dto.Id)
            ?? throw new KeyNotFoundException($"Student with ID {dto.Id} not found.");
        _mapper.Map(dto, student);
        _repository.Update(student);
        await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<StudentDto>(student);
    }

    public async Task DeleteAsync(int id)
    {
        var student = await _repository.GetByIdAsync(id)
            ?? throw new KeyNotFoundException($"Student with ID {id} not found.");
        student.IsDeleted = true;
        student.DeletedAt = DateTime.UtcNow;
        _repository.Update(student);
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<byte[]> ExportToExcelAsync()
    {
        var students = await GetAllAsync();
        using var workbook = new ClosedXML.Excel.XLWorkbook();
        workbook.Worksheets.Add("Students");
        var ws = workbook.Worksheet("Students");
        ws.Cell(1, 1).Value = "Name";
        ws.Cell(1, 2).Value = "Grade";
        ws.Cell(1, 3).Value = "Division";
        ws.Cell(1, 4).Value = "Phone";
        ws.Cell(1, 5).Value = "Parent Phone";
        ws.Cell(1, 6).Value = "Badge Card";
        ws.Cell(1, 7).Value = "Branch";

        ws.Range("A1:G1").Style.Font.Bold = true;
        ws.Range("A1:G1").Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.FromHtml("#16213e");
        ws.Range("A1:G1").Style.Font.FontColor = ClosedXML.Excel.XLColor.White;

        for (int i = 0; i < students.Count; i++)
        {
            ws.Cell(i + 2, 1).Value = students[i].FullName;
            ws.Cell(i + 2, 2).Value = students[i].GradeName;
            ws.Cell(i + 2, 3).Value = students[i].DivisionName;
            ws.Cell(i + 2, 4).Value = students[i].Phone;
            ws.Cell(i + 2, 5).Value = students[i].ParentPhone;
            ws.Cell(i + 2, 6).Value = students[i].BadgeCardNumber;
            ws.Cell(i + 2, 7).Value = students[i].BranchName;
        }
        ws.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
