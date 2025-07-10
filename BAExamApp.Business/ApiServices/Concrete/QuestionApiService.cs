using BAExamApp.Core.Enums;
using BAExamApp.DataAccess.Interfaces.Repositories;
using BAExamApp.Dtos.ApiDtos.QuestionApiDtos;
using BAExamApp.Dtos.Questions;
using BAExamApp.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Business.ApiServices.Concrete;
public class QuestionApiService : IQuestionApiService
{
    private readonly IQuestionRepository _questionRepository;
    private readonly IMapper _mapper;

    public QuestionApiService(IQuestionRepository questionRepository, IMapper mapper)
    {

        _questionRepository = questionRepository;
        _mapper = mapper;
    }
    /// <summary>
    /// sınav kuralında olması istenen özelliklerdeki soruları sorgular ve liste olarak getirir.
    /// </summary>
    /// <param name="questionDifficultyId"> seçilen kuralın içeriğindeki sorunun istenen leveli </param>
    /// <param name="questionType"> seçilen kuralın içeriğindeki sorunun tipi</param>
    /// <param name="subjectId"> seçilen kuralın içeriğindeki subject id'si</param>
    /// <returns> istenen özelliklerdeki soruları listeleyerek döner </returns>
    public async Task<IDataResult<List<QuestionListApiDto>>> GetAllByExamRuleSubtopicAsync(Guid questionDifficultyId, int questionType, List<Guid> subtopicId)
    {
        var questions = await _questionRepository.GetAllAsync(x => x.QuestionDifficultyId == questionDifficultyId && (int)x.QuestionType == questionType && x.QuestionSubtopics.Any(qs => subtopicId.Contains(qs.SubtopicId)) && x.State == State.Approved && x.Status != Status.Passive, true);

        return new SuccessDataResult<List<QuestionListApiDto>>(_mapper.Map<List<QuestionListApiDto>>(questions), Messages.ListedSuccess);
    }
}
