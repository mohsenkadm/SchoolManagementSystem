using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class SalaryService : ISalaryService
{
    private readonly IRepository<SalarySetup> _repository;
    private readonly IRepository<Student> _studentRepo;
    private readonly IRepository<Teacher> _teacherRepo;
    private readonly IRepository<HrEmployee> _staffRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public SalaryService(IRepository<SalarySetup> repository, IRepository<Student> studentRepo,
        IRepository<Teacher> teacherRepo, IRepository<HrEmployee> staffRepo,
        IUnitOfWork unitOfWork, IMapper mapper)
    {
        _repository = repository; _studentRepo = studentRepo;
        _teacherRepo = teacherRepo; _staffRepo = staffRepo;
        _unitOfWork = unitOfWork; _mapper = mapper;
    }

    public async Task<List<SalarySetupDto>> GetAllSetupsAsync()
    {
        var entities = await _repository.Query().Include(s => s.School).ToListAsync();
        var dtos = _mapper.Map<List<SalarySetupDto>>(entities);
        await ResolvePersonNames(entities, dtos);
        return dtos;
    }

    public async Task<List<SalarySetupDto>> GetBySchoolIdAsync(int schoolId)
    {
        var entities = await _repository.Query().Where(s => s.SchoolId == schoolId).Include(s => s.School).ToListAsync();
        var dtos = _mapper.Map<List<SalarySetupDto>>(entities);
        await ResolvePersonNames(entities, dtos);
        return dtos;
    }

    public async Task<SalarySetupDto?> GetByPersonAsync(int personId, PersonType personType, int schoolId)
    {
        var entity = await _repository.Query()
            .Where(s => s.PersonId == personId && s.PersonType == personType && s.SchoolId == schoolId)
            .Include(s => s.School)
            .FirstOrDefaultAsync();
        if (entity == null) return null;
        var dto = _mapper.Map<SalarySetupDto>(entity);
        await ResolvePersonNames(new List<SalarySetup> { entity }, new List<SalarySetupDto> { dto });
        return dto;
    }

    private async Task ResolvePersonNames(List<SalarySetup> entities, List<SalarySetupDto> dtos)
    {
        var teacherIds = entities.Where(e => e.PersonType == PersonType.Teacher).Select(e => e.PersonId).Distinct().ToList();
        var staffIds = entities.Where(e => e.PersonType == PersonType.Staff).Select(e => e.PersonId).Distinct().ToList();

        var teacherNames = teacherIds.Count > 0
            ? await _teacherRepo.Query().Where(t => teacherIds.Contains(t.Id)).ToDictionaryAsync(t => t.Id, t => t.FullName)
            : new Dictionary<int, string>();
        var staffNames = staffIds.Count > 0
            ? await _staffRepo.Query().Where(s => staffIds.Contains(s.Id)).ToDictionaryAsync(s => s.Id, s => s.FullName)
            : new Dictionary<int, string>();

        foreach (var dto in dtos)
        {
            dto.PersonName = dto.PersonType switch
            {
                PersonType.Teacher => teacherNames.GetValueOrDefault(dto.PersonId),
                PersonType.Staff => staffNames.GetValueOrDefault(dto.PersonId),
                _ => null
            };
        }
    }

    public async Task<SalarySetupDto?> GetSetupByIdAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity == null ? null : _mapper.Map<SalarySetupDto>(entity);
    }

    public async Task<SalarySetupDto> CreateSetupAsync(SalarySetupDto dto)
    {
        var entity = _mapper.Map<SalarySetup>(dto); entity.Id = 0;
        await _repository.AddAsync(entity); await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<SalarySetupDto>(entity);
    }

    public async Task<SalarySetupDto> UpdateSetupAsync(SalarySetupDto dto)
    {
        var entity = await _repository.GetByIdAsync(dto.Id) ?? throw new KeyNotFoundException();
        entity.PersonId = dto.PersonId; entity.PersonType = dto.PersonType;
        entity.BaseSalary = dto.BaseSalary; entity.Allowances = dto.Allowances; entity.Deductions = dto.Deductions;
        entity.SchoolId = dto.SchoolId;
        _repository.Update(entity); await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<SalarySetupDto>(entity);
    }

    public async Task DeleteSetupAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id) ?? throw new KeyNotFoundException();
        entity.IsDeleted = true; entity.DeletedAt = DateTime.UtcNow;
        _repository.Update(entity); await _unitOfWork.SaveChangesAsync();
    }

    public async Task<byte[]> ExportToExcelAsync()
    {
        var items = await GetAllSetupsAsync();
        using var workbook = new ClosedXML.Excel.XLWorkbook();
        workbook.Worksheets.Add("Salaries");
        var ws = workbook.Worksheet("Salaries");
        ws.Cell(1, 1).Value = "Person"; ws.Cell(1, 2).Value = "Type"; ws.Cell(1, 3).Value = "Base Salary";
        ws.Cell(1, 4).Value = "Allowances"; ws.Cell(1, 5).Value = "Deductions"; ws.Cell(1, 6).Value = "Net Salary";
        ws.Range("A1:F1").Style.Font.Bold = true;
        for (int i = 0; i < items.Count; i++)
        {
            ws.Cell(i + 2, 1).Value = items[i].PersonName;
            ws.Cell(i + 2, 2).Value = items[i].PersonType.ToString();
            ws.Cell(i + 2, 3).Value = items[i].BaseSalary;
            ws.Cell(i + 2, 4).Value = items[i].Allowances;
            ws.Cell(i + 2, 5).Value = items[i].Deductions;
            ws.Cell(i + 2, 6).Value = items[i].NetSalary;
        }
        ws.Columns().AdjustToContents();
        using var stream = new MemoryStream();
        workbook.SaveAs(stream); return stream.ToArray();
    }
}

