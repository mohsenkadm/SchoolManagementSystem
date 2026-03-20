using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class AttendanceService : IAttendanceService
{
    private readonly IRepository<Attendance> _attendanceRepo;
    private readonly IRepository<Student> _studentRepo;
    private readonly IRepository<Teacher> _teacherRepo;
    private readonly IRepository<Staff> _staffRepo;
    private readonly IRepository<Branch> _branchRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public AttendanceService(IRepository<Attendance> attendanceRepo, IRepository<Student> studentRepo,
        IRepository<Teacher> teacherRepo, IRepository<Staff> staffRepo,
        IRepository<Branch> branchRepo, IUnitOfWork unitOfWork, IMapper mapper)
    {
        _attendanceRepo = attendanceRepo; _studentRepo = studentRepo;
        _teacherRepo = teacherRepo; _staffRepo = staffRepo;
        _branchRepo = branchRepo; _unitOfWork = unitOfWork; _mapper = mapper;
    }

    private async Task<int> ResolveSchoolIdAsync(int branchId)
    {
        var branch = await _branchRepo.Query().FirstOrDefaultAsync(b => b.Id == branchId);
        return branch?.SchoolId ?? 0;
    }

    public async Task<BulkAttendanceItemDto?> ResolveBadgeAsync(string badgeCardNumber)
    {
        var student = await _studentRepo.Query().FirstOrDefaultAsync(s => s.BadgeCardNumber == badgeCardNumber);
        if (student != null)
            return new BulkAttendanceItemDto { PersonId = student.Id, PersonName = student.FullName, PersonType = PersonType.Student, BadgeCardNumber = badgeCardNumber };

        var teacher = await _teacherRepo.Query().FirstOrDefaultAsync(t => t.BadgeCardNumber == badgeCardNumber);
        if (teacher != null)
            return new BulkAttendanceItemDto { PersonId = teacher.Id, PersonName = teacher.FullName, PersonType = PersonType.Teacher, BadgeCardNumber = badgeCardNumber };

        var staff = await _staffRepo.Query().FirstOrDefaultAsync(s => s.BadgeCardNumber == badgeCardNumber);
        if (staff != null)
            return new BulkAttendanceItemDto { PersonId = staff.Id, PersonName = staff.FullName, PersonType = PersonType.Staff, BadgeCardNumber = badgeCardNumber };

        return null;
    }

    public async Task<AttendanceDto> CheckInAsync(CreateAttendanceDto dto)
    {
        var person = await ResolveBadgeAsync(dto.BadgeCardNumber)
            ?? throw new KeyNotFoundException("Badge card not found.");

        var today = DateTime.UtcNow.Date;
        var alreadyExists = await _attendanceRepo.Query().AnyAsync(a =>
            a.PersonId == person.PersonId && a.PersonType == person.PersonType
            && a.AttendanceDate == today && a.Type == dto.Type);
        if (alreadyExists)
        {
            var label = dto.Type == AttendanceType.CheckIn ? "Check-In" : "Check-Out";
            throw new InvalidOperationException($"{person.PersonName} already has a {label} record for today.");
        }

        var schoolId = await ResolveSchoolIdAsync(dto.BranchId);
        var entity = new Attendance
        {
            PersonId = person.PersonId,
            PersonType = person.PersonType,
            BadgeCardNumber = dto.BadgeCardNumber,
            AttendanceDate = today,
            Time = DateTime.UtcNow.TimeOfDay,
            Type = dto.Type,
            BranchId = dto.BranchId,
            SchoolId = schoolId
        };
        await _attendanceRepo.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        var result = _mapper.Map<AttendanceDto>(entity);
        result.PersonName = person.PersonName;
        return result;
    }

    public async Task<List<AttendanceDto>> SaveBulkAsync(BulkAttendanceSaveDto dto)
    {
        var now = DateTime.UtcNow;
        var today = now.Date;
        var schoolId = await ResolveSchoolIdAsync(dto.BranchId);

        // Get existing records for today to detect duplicates
        var existingToday = await _attendanceRepo.Query()
            .Where(a => a.AttendanceDate == today && a.Type == dto.Type)
            .Select(a => new { a.PersonId, a.PersonType })
            .ToListAsync();
        var existingSet = existingToday.Select(e => $"{e.PersonType}_{e.PersonId}").ToHashSet();

        var entities = new List<Attendance>();
        var savedItems = new List<BulkAttendanceItemDto>();
        var skippedNames = new List<string>();

        foreach (var item in dto.Items)
        {
            var key = $"{item.PersonType}_{item.PersonId}";
            if (existingSet.Contains(key))
            {
                skippedNames.Add(item.PersonName ?? "Unknown");
                continue;
            }
            existingSet.Add(key); // prevent duplicates within the same batch

            entities.Add(new Attendance
            {
                PersonId = item.PersonId,
                PersonType = item.PersonType,
                BadgeCardNumber = item.BadgeCardNumber,
                AttendanceDate = today,
                Time = now.TimeOfDay,
                Type = dto.Type,
                BranchId = dto.BranchId,
                SchoolId = schoolId
            });
            savedItems.Add(item);
        }

        if (skippedNames.Count > 0 && entities.Count == 0)
        {
            var label = dto.Type == AttendanceType.CheckIn ? "Check-In" : "Check-Out";
            throw new InvalidOperationException($"All persons already have a {label} record for today: {string.Join(", ", skippedNames)}");
        }

        foreach (var e in entities)
            await _attendanceRepo.AddAsync(e);
        await _unitOfWork.SaveChangesAsync();

        var dtos = _mapper.Map<List<AttendanceDto>>(entities);
        for (int i = 0; i < dtos.Count; i++)
            dtos[i].PersonName = savedItems[i].PersonName;
        return dtos;
    }

    public async Task<AttendanceReportDto> GetReportAsync(AttendanceFilterDto filter)
    {
        var query = BuildFilteredQuery(filter);
        var items = await query.OrderByDescending(a => a.AttendanceDate).ThenByDescending(a => a.Time).ToListAsync();
        var dtos = _mapper.Map<List<AttendanceDto>>(items);
        await ResolvePersonNamesAsync(items, dtos);

        return new AttendanceReportDto
        {
            TotalRecords = dtos.Count,
            CheckInCount = dtos.Count(d => d.Type == AttendanceType.CheckIn),
            CheckOutCount = dtos.Count(d => d.Type == AttendanceType.CheckOut),
            AbsentCount = dtos.Count(d => d.Type == AttendanceType.Absent),
            StudentCount = dtos.Count(d => d.PersonType == PersonType.Student),
            TeacherCount = dtos.Count(d => d.PersonType == PersonType.Teacher),
            StaffCount = dtos.Count(d => d.PersonType == PersonType.Staff),
            Records = dtos
        };
    }

    public async Task<DataTableResponse<AttendanceDto>> GetDataTableAsync(DataTableRequest request)
    {
        var query = _attendanceRepo.Query().Include(a => a.Branch).Include(a => a.School).AsQueryable();
        if (request.BranchId.HasValue) query = query.Where(a => a.BranchId == request.BranchId);
        if (request.SchoolId.HasValue) query = query.Where(a => a.SchoolId == request.SchoolId);

        if (!string.IsNullOrWhiteSpace(request.SearchValue))
            query = query.Where(a => a.BadgeCardNumber!.Contains(request.SearchValue));

        var totalRecords = await query.CountAsync();
        var items = await query.OrderByDescending(a => a.AttendanceDate).ThenByDescending(a => a.Time)
            .Skip(request.Start).Take(request.Length).ToListAsync();
        var dtos = _mapper.Map<List<AttendanceDto>>(items);
        await ResolvePersonNamesAsync(items, dtos);

        return new DataTableResponse<AttendanceDto>
        {
            Draw = request.Draw, RecordsTotal = totalRecords,
            RecordsFiltered = totalRecords, Data = dtos
        };
    }

    public async Task<byte[]> ExportToExcelAsync(AttendanceFilterDto? filter = null)
    {
        List<AttendanceDto> dtos;
        if (filter != null)
        {
            var report = await GetReportAsync(filter);
            dtos = report.Records;
        }
        else
        {
            var items = await _attendanceRepo.Query().Include(a => a.Branch).Include(a => a.School)
                .OrderByDescending(a => a.AttendanceDate).ToListAsync();
            dtos = _mapper.Map<List<AttendanceDto>>(items);
            await ResolvePersonNamesAsync(items, dtos);
        }

        using var workbook = new ClosedXML.Excel.XLWorkbook();
        workbook.Worksheets.Add("Attendance");
        var ws = workbook.Worksheet("Attendance");
        ws.Cell(1, 1).Value = "Name"; ws.Cell(1, 2).Value = "Person Type"; ws.Cell(1, 3).Value = "Badge";
        ws.Cell(1, 4).Value = "Type"; ws.Cell(1, 5).Value = "Date"; ws.Cell(1, 6).Value = "Time";
        ws.Cell(1, 7).Value = "Branch"; ws.Cell(1, 8).Value = "School";
        ws.Range("A1:H1").Style.Font.Bold = true;
        ws.Range("A1:H1").Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.FromHtml("#16213e");
        ws.Range("A1:H1").Style.Font.FontColor = ClosedXML.Excel.XLColor.White;
        for (int i = 0; i < dtos.Count; i++)
        {
            ws.Cell(i + 2, 1).Value = dtos[i].PersonName;
            ws.Cell(i + 2, 2).Value = dtos[i].PersonType.ToString();
            ws.Cell(i + 2, 3).Value = dtos[i].BadgeCardNumber;
            ws.Cell(i + 2, 4).Value = dtos[i].Type.ToString();
            ws.Cell(i + 2, 5).Value = dtos[i].AttendanceDate.ToString("yyyy-MM-dd");
            ws.Cell(i + 2, 6).Value = dtos[i].Time.ToString(@"hh\:mm");
            ws.Cell(i + 2, 7).Value = dtos[i].BranchName;
            ws.Cell(i + 2, 8).Value = dtos[i].SchoolName;
        }
        ws.Columns().AdjustToContents();
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public async Task<int> MarkAbsenteesAsync(int branchId, DateTime date)
    {
        var dateOnly = date.Date;
        var schoolId = await ResolveSchoolIdAsync(branchId);

        // Get all people who already have a record for this date+branch
        var existingRecords = await _attendanceRepo.Query()
            .Where(a => a.BranchId == branchId && a.AttendanceDate == dateOnly)
            .Select(a => new { a.PersonId, a.PersonType })
            .Distinct().ToListAsync();
        var existingSet = existingRecords.Select(r => $"{r.PersonType}_{r.PersonId}").ToHashSet();

        // Get all active people in this branch
        var students = await _studentRepo.Query().Where(s => s.BranchId == branchId)
            .Select(s => new { s.Id, Type = PersonType.Student }).ToListAsync();
        var teachers = await _teacherRepo.Query().Where(t => t.BranchId == branchId)
            .Select(t => new { t.Id, Type = PersonType.Teacher }).ToListAsync();
        var staff = await _staffRepo.Query().Where(s => s.BranchId == branchId)
            .Select(s => new { s.Id, Type = PersonType.Staff }).ToListAsync();

        var allPeople = students.Select(s => (s.Id, s.Type))
            .Concat(teachers.Select(t => (t.Id, t.Type)))
            .Concat(staff.Select(s => (s.Id, s.Type)));

        int count = 0;
        foreach (var (id, type) in allPeople)
        {
            if (existingSet.Contains($"{type}_{id}")) continue;

            await _attendanceRepo.AddAsync(new Attendance
            {
                PersonId = id,
                PersonType = type,
                AttendanceDate = dateOnly,
                Time = TimeSpan.Zero,
                Type = AttendanceType.Absent,
                BranchId = branchId,
                SchoolId = schoolId,
                IsAutoAbsent = true
            });
            count++;
        }
        if (count > 0) await _unitOfWork.SaveChangesAsync();
        return count;
    }

    private IQueryable<Attendance> BuildFilteredQuery(AttendanceFilterDto filter)
    {
        var query = _attendanceRepo.Query().Include(a => a.Branch).Include(a => a.School).AsQueryable();
        if (filter.DateFrom.HasValue) query = query.Where(a => a.AttendanceDate >= filter.DateFrom.Value.Date);
        if (filter.DateTo.HasValue) query = query.Where(a => a.AttendanceDate <= filter.DateTo.Value.Date);
        if (filter.Type.HasValue) query = query.Where(a => a.Type == filter.Type.Value);
        if (filter.PersonType.HasValue) query = query.Where(a => a.PersonType == filter.PersonType.Value);
        if (filter.BranchId.HasValue) query = query.Where(a => a.BranchId == filter.BranchId.Value);
        if (filter.SchoolId.HasValue) query = query.Where(a => a.SchoolId == filter.SchoolId.Value);
        if (!string.IsNullOrWhiteSpace(filter.SearchValue))
            query = query.Where(a => a.BadgeCardNumber!.Contains(filter.SearchValue));
        return query;
    }

    private async Task ResolvePersonNamesAsync(List<Attendance> items, List<AttendanceDto> dtos)
    {
        var studentIds = items.Where(a => a.PersonType == PersonType.Student).Select(a => a.PersonId).Distinct().ToList();
        var teacherIds = items.Where(a => a.PersonType == PersonType.Teacher).Select(a => a.PersonId).Distinct().ToList();
        var staffIds = items.Where(a => a.PersonType == PersonType.Staff).Select(a => a.PersonId).Distinct().ToList();

        var studentNames = studentIds.Count > 0
            ? await _studentRepo.Query().Where(s => studentIds.Contains(s.Id)).ToDictionaryAsync(s => s.Id, s => s.FullName)
            : new Dictionary<int, string>();
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
                PersonType.Student => studentNames.GetValueOrDefault(dto.PersonId),
                PersonType.Teacher => teacherNames.GetValueOrDefault(dto.PersonId),
                PersonType.Staff => staffNames.GetValueOrDefault(dto.PersonId),
                _ => null
            };
        }
    }

    public async Task<List<AttendanceDto>> GetByPersonAsync(int personId, PersonType personType, int schoolId)
    {
        var items = await _attendanceRepo.Query()
            .Include(a => a.Branch).Include(a => a.School)
            .Where(a => a.PersonId == personId && a.PersonType == personType && a.SchoolId == schoolId)
            .OrderByDescending(a => a.AttendanceDate).ThenByDescending(a => a.Time)
            .ToListAsync();
        var dtos = _mapper.Map<List<AttendanceDto>>(items);
        await ResolvePersonNamesAsync(items, dtos);
        return dtos;
    }

    public async Task<List<AttendanceDto>> GetByParentChildrenAsync(int parentId, int schoolId)
    {
        var childrenIds = await _studentRepo.Query()
            .Where(s => s.ParentId == parentId && s.SchoolId == schoolId)
            .Select(s => s.Id)
            .ToListAsync();

        if (childrenIds.Count == 0) return new List<AttendanceDto>();

        var items = await _attendanceRepo.Query()
            .Include(a => a.Branch).Include(a => a.School)
            .Where(a => childrenIds.Contains(a.PersonId) && a.PersonType == PersonType.Student && a.SchoolId == schoolId)
            .OrderByDescending(a => a.AttendanceDate).ThenByDescending(a => a.Time)
            .ToListAsync();
        var dtos = _mapper.Map<List<AttendanceDto>>(items);
        await ResolvePersonNamesAsync(items, dtos);
        return dtos;
    }
}