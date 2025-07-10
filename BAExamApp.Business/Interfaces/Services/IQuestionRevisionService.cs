using BAExamApp.Dtos.QuestionArranges;
using BAExamApp.Dtos.QuestionRevisions;

namespace BAExamApp.Business.Interfaces.Services;
public interface IQuestionRevisionService
{
    Task<List<QuestionRevisionListDto>> GetAllAsync();
    Task<IResult> AddAsync(QuestionRevisionCreateDto questionRevisionCreateDto);
    Task<QuestionRevisionDto> GetByIdAsync(Guid id);
    Task<IResult> DeleteAsync(Guid id);
    Task<IDataResult<QuestionRevisionDto>> UpdateAsync(QuestionRevisionUpdateDto questionRevisionUpdateDto);
    Task<List<QuestionRevisionListDto>> GetAllRevisionsByQuestionId(Guid questionId);
    Task<QuestionRevisionDto> GetActiveByQuestionId(Guid questionId);
    Task<bool> AnyActive(Guid questionId);

    /// <summary>
    /// Onaylanan sorunun id'sine göre o soruya yapılan düzenleme yorumlarını getirir
    /// </summary>
    /// <param name="id"></param>
    /// <returns>QuestionArrangeListDto listesi döner</returns>
    Task<List<QuestionArrangeListDto>> GetAllArrangesByQuestionIdAsync(Guid id);
    /// <summary>
    /// Onaylanan Sorular için yapılan düzenleme yorumlarının databaseye eklenmesini sağlar.
    /// </summary>
    /// <param name="questionFeedbackCreate"></param>
    /// <returns>QuestionArrangeDto</returns>
    Task<IDataResult<QuestionArrangeDto>> AddArrangeAsync(QuestionArrangeCreateDto questionArrangeCreate);
}
