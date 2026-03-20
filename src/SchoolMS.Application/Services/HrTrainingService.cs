using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class HrTrainingService : IHrTrainingService
{
    private readonly IRepository<HrTrainingProgram> _programRepo;
    private readonly IRepository<HrTrainingRecord> _recordRepo;
    private readonly IRepository<HrTrainingRequest> _requestRepo;
    private readonly IRepository<HrProfessionalCertificate> _certRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public HrTrainingService(IRepository<HrTrainingProgram> programRepo, IRepository<HrTrainingRecord> recordRepo,
        IRepository<HrTrainingRequest> requestRepo, IRepository<HrProfessionalCertificate> certRepo,
        IUnitOfWork unitOfWork, IMapper mapper)
    {
        _programRepo = programRepo; _recordRepo = recordRepo; _requestRepo = requestRepo;
        _certRepo = certRepo; _unitOfWork = unitOfWork; _mapper = mapper;
    }

    public async Task<List<HrTrainingProgramDto>> GetProgramsAsync()
    {
        var items = await _programRepo.Query().Include(p => p.Participants).OrderByDescending(p => p.StartDate).ToListAsync();
        return items.Select(p => new HrTrainingProgramDto
        {
            Id = p.Id, ProgramName = p.ProgramName, ProgramNameAr = p.ProgramNameAr,
            Description = p.Description, Category = p.Category, Provider = p.Provider,
            Trainer = p.Trainer, Location = p.Location, StartDate = p.StartDate, EndDate = p.EndDate,
            DurationHours = p.DurationHours, MaxParticipants = p.MaxParticipants, Cost = p.Cost,
            Status = p.Status, IsMandatory = p.IsMandatory, Notes = p.Notes,
            ParticipantCount = p.Participants.Count
        }).ToList();
    }

    public async Task<HrTrainingProgramDto?> GetProgramByIdAsync(int id)
    {
        var p = await _programRepo.Query().Include(p => p.Participants).FirstOrDefaultAsync(p => p.Id == id);
        return p == null ? null : _mapper.Map<HrTrainingProgramDto>(p);
    }

    public async Task<HrTrainingProgramDto> CreateProgramAsync(HrTrainingProgramDto dto)
    {
        var entity = _mapper.Map<HrTrainingProgram>(dto); entity.Id = 0;
        await _programRepo.AddAsync(entity); await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<HrTrainingProgramDto>(entity);
    }

    public async Task<HrTrainingProgramDto> UpdateProgramAsync(HrTrainingProgramDto dto)
    {
        var entity = await _programRepo.GetByIdAsync(dto.Id) ?? throw new KeyNotFoundException($"Program {dto.Id} not found.");
        entity.ProgramName = dto.ProgramName; entity.ProgramNameAr = dto.ProgramNameAr;
        entity.Description = dto.Description; entity.Category = dto.Category;
        entity.Provider = dto.Provider; entity.Trainer = dto.Trainer; entity.Location = dto.Location;
        entity.StartDate = dto.StartDate; entity.EndDate = dto.EndDate;
        entity.DurationHours = dto.DurationHours; entity.MaxParticipants = dto.MaxParticipants;
        entity.Cost = dto.Cost; entity.Status = dto.Status;
        entity.IsMandatory = dto.IsMandatory; entity.Notes = dto.Notes;
        _programRepo.Update(entity); await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<HrTrainingProgramDto>(entity);
    }

    public async Task DeleteProgramAsync(int id)
    {
        var entity = await _programRepo.GetByIdAsync(id) ?? throw new KeyNotFoundException($"Program {id} not found.");
        entity.IsDeleted = true; entity.DeletedAt = DateTime.UtcNow;
        _programRepo.Update(entity); await _unitOfWork.SaveChangesAsync();
    }

    public async Task<List<HrTrainingRecordDto>> GetRecordsAsync(int? programId = null, int? employeeId = null)
    {
        var query = _recordRepo.Query().Include(r => r.Employee).Include(r => r.TrainingProgram).AsQueryable();
        if (programId.HasValue) query = query.Where(r => r.TrainingProgramId == programId.Value);
        if (employeeId.HasValue) query = query.Where(r => r.EmployeeId == employeeId.Value);
        var items = await query.ToListAsync();
        return items.Select(r => new HrTrainingRecordDto
        {
            Id = r.Id, EmployeeId = r.EmployeeId, EmployeeName = r.Employee?.FullName,
            TrainingProgramId = r.TrainingProgramId, ProgramName = r.TrainingProgram?.ProgramName,
            Status = r.Status, Score = r.Score, CertificateIssued = r.CertificateIssued,
            CertificateNumber = r.CertificateNumber, Feedback = r.Feedback, Rating = r.Rating
        }).ToList();
    }

    public async Task<HrTrainingRecordDto> EnrollEmployeeAsync(HrTrainingRecordDto dto)
    {
        var entity = _mapper.Map<HrTrainingRecord>(dto); entity.Id = 0;
        entity.Status = TrainingParticipantStatus.Enrolled;
        await _recordRepo.AddAsync(entity); await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<HrTrainingRecordDto>(entity);
    }

    public async Task<HrTrainingRecordDto> UpdateRecordAsync(HrTrainingRecordDto dto)
    {
        var entity = await _recordRepo.GetByIdAsync(dto.Id) ?? throw new KeyNotFoundException($"Record {dto.Id} not found.");
        entity.Status = dto.Status; entity.Score = dto.Score;
        entity.CertificateIssued = dto.CertificateIssued; entity.CertificateNumber = dto.CertificateNumber;
        entity.Feedback = dto.Feedback; entity.Rating = dto.Rating;
        _recordRepo.Update(entity); await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<HrTrainingRecordDto>(entity);
    }

    public async Task<List<HrTrainingRequestDto>> GetRequestsAsync(TrainingRequestStatus? status = null)
    {
        var query = _requestRepo.Query().Include(r => r.Employee).AsQueryable();
        if (status.HasValue) query = query.Where(r => r.Status == status.Value);
        var items = await query.OrderByDescending(r => r.CreatedAt).ToListAsync();
        return _mapper.Map<List<HrTrainingRequestDto>>(items);
    }

    public async Task<HrTrainingRequestDto> CreateRequestAsync(HrTrainingRequestDto dto)
    {
        var entity = _mapper.Map<HrTrainingRequest>(dto); entity.Id = 0;
        entity.Status = TrainingRequestStatus.Pending;
        await _requestRepo.AddAsync(entity); await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<HrTrainingRequestDto>(entity);
    }

    public async Task ApproveRequestAsync(int id, string approvedBy)
    {
        var entity = await _requestRepo.GetByIdAsync(id) ?? throw new KeyNotFoundException($"Request {id} not found.");
        entity.Status = TrainingRequestStatus.Approved; entity.ApprovedBy = approvedBy;
        _requestRepo.Update(entity); await _unitOfWork.SaveChangesAsync();
    }

    public async Task RejectRequestAsync(int id, string rejectedBy)
    {
        var entity = await _requestRepo.GetByIdAsync(id) ?? throw new KeyNotFoundException($"Request {id} not found.");
        entity.Status = TrainingRequestStatus.Rejected; entity.ApprovedBy = rejectedBy;
        _requestRepo.Update(entity); await _unitOfWork.SaveChangesAsync();
    }

    public async Task<List<HrProfessionalCertificateDto>> GetCertificatesAsync(int employeeId)
    {
        var items = await _certRepo.Query().Where(c => c.EmployeeId == employeeId).ToListAsync();
        return _mapper.Map<List<HrProfessionalCertificateDto>>(items);
    }

    public async Task<HrProfessionalCertificateDto> CreateCertificateAsync(HrProfessionalCertificateDto dto)
    {
        var entity = _mapper.Map<HrProfessionalCertificate>(dto); entity.Id = 0;
        await _certRepo.AddAsync(entity); await _unitOfWork.SaveChangesAsync();
        return _mapper.Map<HrProfessionalCertificateDto>(entity);
    }

    public async Task DeleteCertificateAsync(int id)
    {
        var entity = await _certRepo.GetByIdAsync(id) ?? throw new KeyNotFoundException($"Certificate {id} not found.");
        entity.IsDeleted = true; entity.DeletedAt = DateTime.UtcNow;
        _certRepo.Update(entity); await _unitOfWork.SaveChangesAsync();
    }
}
