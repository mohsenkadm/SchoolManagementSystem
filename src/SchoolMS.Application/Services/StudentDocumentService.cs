using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class StudentDocumentService : IStudentDocumentService
{
    private readonly IRepository<StudentDocument> _repository;
    private readonly IUnitOfWork _unitOfWork;

    public StudentDocumentService(IRepository<StudentDocument> repository, IUnitOfWork unitOfWork)
    { _repository = repository; _unitOfWork = unitOfWork; }

    public async Task<List<StudentDocumentDto>> GetAllAsync()
    {
        var items = await _repository.Query().Include(d => d.Student).ToListAsync();
        return items.Select(d => new StudentDocumentDto
        {
            Id = d.Id, StudentId = d.StudentId, StudentName = d.Student?.FullName, DocumentName = d.DocumentName,
            DocumentType = d.DocumentType, FilePath = d.FilePath, FileType = d.FileType,
            FileSize = d.FileSize, UploadDate = d.UploadDate, Notes = d.Notes
        }).ToList();
    }

    public async Task<List<StudentDocumentDto>> GetByStudentIdAsync(int studentId)
    {
        var items = await _repository.Query().Where(d => d.StudentId == studentId).Include(d => d.Student).ToListAsync();
        return items.Select(d => new StudentDocumentDto
        {
            Id = d.Id, StudentId = d.StudentId, StudentName = d.Student?.FullName, DocumentName = d.DocumentName,
            DocumentType = d.DocumentType, FilePath = d.FilePath, FileType = d.FileType,
            FileSize = d.FileSize, UploadDate = d.UploadDate, Notes = d.Notes
        }).ToList();
    }

    public async Task<StudentDocumentDto> CreateAsync(StudentDocumentDto dto)
    {
        var entity = new StudentDocument
        {
            StudentId = dto.StudentId, DocumentName = dto.DocumentName, DocumentType = dto.DocumentType,
            FilePath = dto.FilePath, FileType = dto.FileType, FileSize = dto.FileSize,
            UploadDate = DateTime.UtcNow, Notes = dto.Notes
        };
        await _repository.AddAsync(entity); await _unitOfWork.SaveChangesAsync();
        dto.Id = entity.Id; return dto;
    }

    public async Task DeleteAsync(int id)
    {
        var e = await _repository.GetByIdAsync(id) ?? throw new KeyNotFoundException();
        e.IsDeleted = true; e.DeletedAt = DateTime.UtcNow;
        _repository.Update(e); await _unitOfWork.SaveChangesAsync();
    }
}

