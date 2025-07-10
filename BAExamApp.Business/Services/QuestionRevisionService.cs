using BAExamApp.Dtos.QuestionArranges;
using BAExamApp.Dtos.QuestionRevisions;
using BAExamApp.Dtos.Students;
using BAExamApp.Dtos.Subjects;
using BAExamApp.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Business.Services;
public class QuestionRevisionService : IQuestionRevisionService
{
    IQuestionRevisionRepository _questionRevisionRepository;
    IQuestionRepository _questionRepository;
    IMapper _mapper;

    public QuestionRevisionService(IQuestionRevisionRepository questionRevisionRepository, IMapper mapper,IQuestionRepository questionRepository)
    {
        _questionRevisionRepository = questionRevisionRepository;
        _questionRepository = questionRepository;
        _mapper = mapper;
    }

    public async Task<IResult> AddAsync(QuestionRevisionCreateDto questionRevisionCreateDto)
    {
        await _questionRevisionRepository.AddAsync(_mapper.Map<QuestionRevision>(questionRevisionCreateDto));
        var question = await _questionRepository.GetByIdAsync(questionRevisionCreateDto.QuestionId);
        question.State = Entities.Enums.State.Reviewed;
        await _questionRepository.UpdateAsync(question);
        await _questionRevisionRepository.SaveChangesAsync();
        var questionRevisiondto = _mapper.Map<QuestionRevisionDto>(questionRevisionCreateDto);
        return new SuccessDataResult<QuestionRevisionDto>(questionRevisiondto, Messages.AddSuccess);
    }

    public async Task<IResult> DeleteAsync(Guid id)
    {
        var questionRevision = await _questionRevisionRepository.GetByIdAsync(id);
        if (questionRevision is null)
        {
            return new ErrorDataResult<QuestionRevisionDto>();
        }
        else
        {
            await _questionRevisionRepository.DeleteAsync(questionRevision);
            await _questionRevisionRepository.SaveChangesAsync();
        }
        return new SuccessDataResult<QuestionRevisionDto>();
    }

    public async Task<List<QuestionRevisionListDto>> GetAllAsync()
    {
        return _mapper.Map<List<QuestionRevisionListDto>>(await _questionRevisionRepository.GetAllAsync(x=>x.RevisionType==Entities.Enums.QuestionRevisionType.Revision));
    }

    public async Task<QuestionRevisionDto> GetByIdAsync(Guid id)
    {
        return _mapper.Map<QuestionRevisionDto>(await _questionRevisionRepository.GetByIdAsync(id));
    }

    public async Task<List<QuestionRevisionListDto>> GetAllRevisionsByQuestionId(Guid questionId) 
    {
        return _mapper.Map<List<QuestionRevisionListDto>>(await _questionRevisionRepository.GetAllAsync(x => x.QuestionId == questionId && x.RevisionType == Entities.Enums.QuestionRevisionType.Revision));
    }

    public async Task<QuestionRevisionDto> GetActiveByQuestionId(Guid questionId)
    {
        var questionRevisionResult = await _questionRevisionRepository.GetAsync(x => x.QuestionId == questionId && x.Status == Core.Enums.Status.Active && x.RevisionType == Entities.Enums.QuestionRevisionType.Revision);
        return _mapper.Map<QuestionRevisionDto>(questionRevisionResult);
    }

    public async Task<IDataResult<QuestionRevisionDto>> UpdateAsync(QuestionRevisionUpdateDto questionRevisionUpdateDto)
    {
        var questionRevision = await _questionRevisionRepository.GetByIdAsync(questionRevisionUpdateDto.Id);
        var updatedRevision = _mapper.Map(questionRevisionUpdateDto, questionRevision);
        await _questionRevisionRepository.UpdateAsync(_mapper.Map<QuestionRevision>(updatedRevision));
        await _questionRevisionRepository.SaveChangesAsync();
        return new SuccessDataResult<QuestionRevisionDto>(_mapper.Map<QuestionRevisionDto>(updatedRevision), Messages.UpdateSuccess);
    }

    public async Task<IDataResult<QuestionRevisionDto>> GetActive()
    {
        var questionRevision = await _questionRevisionRepository.GetActive();
        if (questionRevision is null)
        {
            return new ErrorDataResult<QuestionRevisionDto>(Messages.NullData);
        }

        return new SuccessDataResult<QuestionRevisionDto>(Messages.FoundSuccess);
    }

    public async Task<bool> AnyActive(Guid questionId)
    {
       return await _questionRevisionRepository.AnyAsync(x => x.Status == Core.Enums.Status.Added && x.QuestionId==questionId && x.RevisionType == Entities.Enums.QuestionRevisionType.Revision);
    }

    /// <summary>
    /// Onaylanan sorunun id'sine göre o soruya yapılan düzenleme yorumlarını getirir
    /// </summary>
    /// <param name="id">Düzenleme yapılan sorunun id'si</param>
    /// <returns></returns>

    public async Task<List<QuestionArrangeListDto>> GetAllArrangesByQuestionIdAsync(Guid id)
    {
        var questionArranges = await _questionRevisionRepository.GetAllAsync(x =>
            x.QuestionId == id &&
            x.RevisionType == QuestionRevisionType.Arrange
        );

        return _mapper.Map<List<QuestionArrangeListDto>>(questionArranges);
    }

    /// <summary>
    /// Onaylanan Sorular için yapılan düzenleme yorumlarının databaseye eklenmesini sağlar.
    /// </summary>
    /// <param name="questionArrangeCreate"></param>
    /// <returns></returns>

    public async Task<IDataResult<QuestionArrangeDto>> AddArrangeAsync(QuestionArrangeCreateDto questionArrangeCreate)
    {
        var questionArrange = new QuestionRevision
        {
            QuestionId = questionArrangeCreate.QuestionId,
            Comment = questionArrangeCreate.Comment,
            RequesterAdminId = questionArrangeCreate.ArrangerAdminId,
            RevisionType = QuestionRevisionType.Arrange
        };

        await _questionRevisionRepository.AddAsync(questionArrange);
        await _questionRevisionRepository.SaveChangesAsync();


        return new SuccessDataResult<QuestionArrangeDto>(_mapper.Map<QuestionArrangeDto>(questionArrangeCreate), Messages.AddSuccess);
    }
}
