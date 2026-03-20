using SchoolMS.Application.DTOs;
using SchoolMS.Domain.Enums;

namespace SchoolMS.Application.Interfaces;

public interface IQuizService
{
    Task<List<QuizGroupDto>> GetAllGroupsAsync();
    Task<List<QuizGroupDto>> GetBySchoolIdAsync(int schoolId, int? branchId = null, int? teacherId = null);
    Task<List<QuizGroupDto>> GetByClassRoomIdsAsync(List<int> classRoomIds, int schoolId);
    Task<QuizGroupDto?> GetGroupByIdAsync(int id);
    Task<QuizGroupDto> CreateGroupAsync(QuizGroupDto dto);
    Task DeleteGroupAsync(int id);
    Task<List<QuizQuestionDto>> GetQuestionsByGroupIdAsync(int groupId);
    Task<QuizQuestionDto> AddQuestionAsync(QuizQuestionDto dto);
    Task DeleteQuestionAsync(int id);
    Task<List<QuizAnswerDto>> SubmitAnswersAsync(int quizGroupId, int studentId, List<SubmitQuizAnswerDto> answers, int schoolId);
    Task<List<QuizAnswerDto>> GetStudentAnswersAsync(int quizGroupId, int studentId);
}

