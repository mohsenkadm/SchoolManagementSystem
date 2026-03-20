using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Application.DTOs;
using SchoolMS.Application.Interfaces;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Enums;
using SchoolMS.Domain.Interfaces;

namespace SchoolMS.Application.Services;

public class QuizService : IQuizService
{
    private readonly IRepository<QuizGroup> _groupRepo;
    private readonly IRepository<QuizQuestion> _questionRepo;
    private readonly IRepository<QuizAnswer> _answerRepo;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public QuizService(IRepository<QuizGroup> groupRepo, IRepository<QuizQuestion> questionRepo,
        IRepository<QuizAnswer> answerRepo, IUnitOfWork unitOfWork, IMapper mapper)
    { _groupRepo = groupRepo; _questionRepo = questionRepo; _answerRepo = answerRepo; _unitOfWork = unitOfWork; _mapper = mapper; }

    public async Task<List<QuizGroupDto>> GetAllGroupsAsync()
    {
        var items = await _groupRepo.Query()
            .Include(q => q.ClassRoom).ThenInclude(c => c.Grade)
            .Include(q => q.ClassRoom).ThenInclude(c => c.Division)
            .Include(q => q.Subject).Include(q => q.Teacher)
            .ToListAsync();

        var groupIds = items.Select(q => q.Id).ToList();
        var questionCounts = await _questionRepo.Query()
            .IgnoreQueryFilters()
            .Where(q => !q.IsDeleted && groupIds.Contains(q.QuizGroupId))
            .GroupBy(q => q.QuizGroupId)
            .Select(g => new { GroupId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.GroupId, x => x.Count);

        return items.Select(q => new QuizGroupDto
        {
            Id = q.Id, GroupName = q.GroupName, ClassRoomId = q.ClassRoomId,
            GradeName = q.ClassRoom?.Grade?.GradeName, DivisionName = q.ClassRoom?.Division?.DivisionName,
            SubjectId = q.SubjectId, SubjectName = q.Subject?.SubjectName,
            TeacherId = q.TeacherId, TeacherName = q.Teacher?.FullName,
            AcademicYearId = q.AcademicYearId,
            QuestionCount = questionCounts.GetValueOrDefault(q.Id, 0)
        }).ToList();
    }

    public async Task<QuizGroupDto?> GetGroupByIdAsync(int id)
    {
        var q = await _groupRepo.Query()
            .Include(q => q.ClassRoom).ThenInclude(c => c.Grade)
            .Include(q => q.ClassRoom).ThenInclude(c => c.Division)
            .Include(q => q.Subject).Include(q => q.Teacher)
            .FirstOrDefaultAsync(q => q.Id == id);
        if (q == null) return null;

        var questionCount = await _questionRepo.Query()
            .IgnoreQueryFilters()
            .CountAsync(x => !x.IsDeleted && x.QuizGroupId == id);

        return new QuizGroupDto
        {
            Id = q.Id, GroupName = q.GroupName, ClassRoomId = q.ClassRoomId,
            SubjectId = q.SubjectId, TeacherId = q.TeacherId, AcademicYearId = q.AcademicYearId,
            QuestionCount = questionCount
        };
    }

    public async Task<QuizGroupDto> CreateGroupAsync(QuizGroupDto dto)
    {
        var entity = new QuizGroup
        {
            GroupName = dto.GroupName, ClassRoomId = dto.ClassRoomId,
            SubjectId = dto.SubjectId, TeacherId = dto.TeacherId, AcademicYearId = dto.AcademicYearId,
            SchoolId = dto.SchoolId
        };
        await _groupRepo.AddAsync(entity); await _unitOfWork.SaveChangesAsync();
        dto.Id = entity.Id; return dto;
    }

    public async Task DeleteGroupAsync(int id)
    {
        var entity = await _groupRepo.GetByIdAsync(id) ?? throw new KeyNotFoundException();
        entity.IsDeleted = true; entity.DeletedAt = DateTime.UtcNow;
        _groupRepo.Update(entity); await _unitOfWork.SaveChangesAsync();
    }

    public async Task<List<QuizQuestionDto>> GetQuestionsByGroupIdAsync(int groupId)
    {
        var items = await _questionRepo.Query()
            .IgnoreQueryFilters()
            .Where(q => !q.IsDeleted && q.QuizGroupId == groupId)
            .OrderBy(q => q.Id)
            .ToListAsync();
        return items.Select(q => new QuizQuestionDto
        {
            Id = q.Id, QuizGroupId = q.QuizGroupId, QuestionText = q.QuestionText,
            Type = q.Type, Options = q.Options, CorrectAnswer = q.CorrectAnswer, Points = q.Points
        }).ToList();
    }

    public async Task<QuizQuestionDto> AddQuestionAsync(QuizQuestionDto dto)
    {
        var entity = new QuizQuestion
        {
            QuizGroupId = dto.QuizGroupId, QuestionText = dto.QuestionText,
            Type = dto.Type, Options = dto.Options, CorrectAnswer = dto.CorrectAnswer, Points = dto.Points,
            SchoolId = (await _groupRepo.GetByIdAsync(dto.QuizGroupId))!.SchoolId
        };
        await _questionRepo.AddAsync(entity); await _unitOfWork.SaveChangesAsync();
        dto.Id = entity.Id; return dto;
    }

    public async Task DeleteQuestionAsync(int id)
    {
        var entity = await _questionRepo.GetByIdAsync(id) ?? throw new KeyNotFoundException();
        entity.IsDeleted = true; entity.DeletedAt = DateTime.UtcNow;
        _questionRepo.Update(entity); await _unitOfWork.SaveChangesAsync();
    }

    public async Task<List<QuizGroupDto>> GetBySchoolIdAsync(int schoolId, int? branchId = null, int? teacherId = null)
    {
        var query = _groupRepo.Query().Where(q => q.SchoolId == schoolId);
        if (branchId.HasValue) query = query.Where(q => q.ClassRoom.BranchId == branchId.Value);
        if (teacherId.HasValue) query = query.Where(q => q.TeacherId == teacherId.Value);
        var items = await query
            .Include(q => q.ClassRoom).ThenInclude(c => c.Grade)
            .Include(q => q.ClassRoom).ThenInclude(c => c.Division)
            .Include(q => q.Subject).Include(q => q.Teacher)
            .ToListAsync();
        return MapGroups(items, await GetQuestionCounts(items));
    }

    public async Task<List<QuizGroupDto>> GetByClassRoomIdsAsync(List<int> classRoomIds, int schoolId)
    {
        if (classRoomIds.Count == 0) return new List<QuizGroupDto>();
        var items = await _groupRepo.Query()
            .Where(q => q.SchoolId == schoolId && classRoomIds.Contains(q.ClassRoomId))
            .Include(q => q.ClassRoom).ThenInclude(c => c.Grade)
            .Include(q => q.ClassRoom).ThenInclude(c => c.Division)
            .Include(q => q.Subject).Include(q => q.Teacher)
            .ToListAsync();
        return MapGroups(items, await GetQuestionCounts(items));
    }

    public async Task<List<QuizAnswerDto>> SubmitAnswersAsync(int quizGroupId, int studentId, List<SubmitQuizAnswerDto> answers, int schoolId)
    {
        var questions = await _questionRepo.Query()
            .IgnoreQueryFilters()
            .Where(q => !q.IsDeleted && q.QuizGroupId == quizGroupId)
            .ToListAsync();
        var questionMap = questions.ToDictionary(q => q.Id);

        var result = new List<QuizAnswerDto>();
        foreach (var a in answers)
        {
            if (!questionMap.TryGetValue(a.QuizQuestionId, out var question)) continue;
            var isCorrect = string.Equals(a.Answer?.Trim(), question.CorrectAnswer?.Trim(), StringComparison.OrdinalIgnoreCase);
            var entity = new QuizAnswer
            {
                QuizQuestionId = a.QuizQuestionId,
                StudentId = studentId,
                Answer = a.Answer,
                IsCorrect = isCorrect,
                PointsEarned = isCorrect ? question.Points : 0,
                SchoolId = schoolId
            };
            await _answerRepo.AddAsync(entity);
            result.Add(new QuizAnswerDto
            {
                QuizQuestionId = a.QuizQuestionId,
                QuestionText = question.QuestionText,
                StudentId = studentId,
                Answer = a.Answer,
                IsCorrect = isCorrect,
                PointsEarned = entity.PointsEarned
            });
        }
        await _unitOfWork.SaveChangesAsync();
        foreach (var (r, e) in result.Zip(result)) r.Id = r.Id;
        return result;
    }

    public async Task<List<QuizAnswerDto>> GetStudentAnswersAsync(int quizGroupId, int studentId)
    {
        var questionIds = await _questionRepo.Query()
            .IgnoreQueryFilters()
            .Where(q => !q.IsDeleted && q.QuizGroupId == quizGroupId)
            .Select(q => q.Id)
            .ToListAsync();

        var items = await _answerRepo.Query()
            .Where(a => questionIds.Contains(a.QuizQuestionId) && a.StudentId == studentId)
            .Include(a => a.QuizQuestion)
            .Include(a => a.Student)
            .ToListAsync();

        return items.Select(a => new QuizAnswerDto
        {
            Id = a.Id, QuizQuestionId = a.QuizQuestionId,
            QuestionText = a.QuizQuestion?.QuestionText,
            StudentId = a.StudentId, StudentName = a.Student?.FullName,
            Answer = a.Answer, IsCorrect = a.IsCorrect, PointsEarned = a.PointsEarned
        }).ToList();
    }

    private async Task<Dictionary<int, int>> GetQuestionCounts(List<QuizGroup> items)
    {
        var groupIds = items.Select(q => q.Id).ToList();
        return await _questionRepo.Query()
            .IgnoreQueryFilters()
            .Where(q => !q.IsDeleted && groupIds.Contains(q.QuizGroupId))
            .GroupBy(q => q.QuizGroupId)
            .Select(g => new { GroupId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.GroupId, x => x.Count);
    }

    private static List<QuizGroupDto> MapGroups(List<QuizGroup> items, Dictionary<int, int> questionCounts)
    {
        return items.Select(q => new QuizGroupDto
        {
            Id = q.Id, GroupName = q.GroupName, ClassRoomId = q.ClassRoomId,
            GradeName = q.ClassRoom?.Grade?.GradeName, DivisionName = q.ClassRoom?.Division?.DivisionName,
            SubjectId = q.SubjectId, SubjectName = q.Subject?.SubjectName,
            TeacherId = q.TeacherId, TeacherName = q.Teacher?.FullName,
            AcademicYearId = q.AcademicYearId, SchoolId = q.SchoolId,
            QuestionCount = questionCounts.GetValueOrDefault(q.Id, 0)
        }).ToList();
    }
}

