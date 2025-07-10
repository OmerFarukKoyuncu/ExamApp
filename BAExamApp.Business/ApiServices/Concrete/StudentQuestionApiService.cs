using BAExamApp.Dtos.ApiDtos.ExamRuleSubtopicApiDtos;
using BAExamApp.Dtos.ApiDtos.QuestionApiDtos;
using BAExamApp.Dtos.ExamRuleSubtopics;
using BAExamApp.Dtos.Questions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Business.ApiServices.Concrete;
public class StudentQuestionApiService : IStudentQuestionApiService
{
    private readonly IMapper _mapper;
    private readonly IQuestionApiService _questionService;

    public StudentQuestionApiService(IMapper mapper, IQuestionApiService questionService)
    {
        _mapper = mapper;
        _questionService = questionService;
    }
    /// <summary>
    /// Gönderilen sınav kurallarına göre genel soru havuzundan istenen soru seviyesi ve soru tipine göre istenen miktarda rastegele soru seçilerek bir soru listesi oluşturur.
    /// </summary>
    /// <param name="examRuleSubtopic">ExamRuleSubtopic</param>
    /// <returns>DataResult<QuestionForStudentListDto></returns>
    public async Task<IDataResult<List<QuestionListApiDto>>> CreateQuestionPoolForExamRuleSubtopicsAsync(List<ExamRuleSubtopicApiDto> examRuleSubtopics)
    {
        Random rnd = new Random();

        var questionList = new List<QuestionListApiDto>();

        foreach (var examRuleSubtopic in examRuleSubtopics)
        {
            //List<QuestionListDto> questionsFiltered = (await _questionService.GetAllByExamRuleSubtopicAsync(examRuleSubtopic.QuestionDifficultyId, examRuleSubtopic.QuestionType, examRuleSubtopic.SubtopicId)).Data;

            List<Guid> subtopicIds = new List<Guid> { examRuleSubtopic.SubtopicId };
            List<QuestionListApiDto> questionsFiltered = (await _questionService.GetAllByExamRuleSubtopicAsync(examRuleSubtopic.QuestionDifficultyId, examRuleSubtopic.QuestionType, subtopicIds)).Data;

            if (questionsFiltered.Count < examRuleSubtopic.QuestionCount)
            {
                return new ErrorDataResult<List<QuestionListApiDto>>(Messages.PleaseAddQuestionsBefore);
            }

            for (var i = 0; i < examRuleSubtopic.QuestionCount; i++)
            {
                var question = questionsFiltered[rnd.Next(questionsFiltered.Count)];

                questionsFiltered.Remove(question);

                questionList.Add(question);
            }
        }

        return new SuccessDataResult<List<QuestionListApiDto>>(questionList, Messages.AddSuccess);
    }
    /// <summary>
    /// Gönderilen alt konuların sorularının toplam sürelerini döndürür.
    /// </summary>
    /// <param name="subtopics"></param>
    /// <returns></returns>
    public async Task<IDataResult<string>> TotalGivenTimeAsync(IEnumerable<ExamRuleSubtopicApiDto> subtopics)
    {

        TimeSpan totalTimeGiven = TimeSpan.Zero;

        foreach (var subtopic in subtopics)
        {

            var questionListResult = await CreateQuestionPoolForExamRuleSubtopicsAsync(new List<ExamRuleSubtopicApiDto> { subtopic });

            if (questionListResult == null || !questionListResult.IsSuccess)
            {
                return new ErrorDataResult<string>(string.Empty, Messages.QuestionPoolCreationFailed);
            }

            foreach (var question in questionListResult.Data)
            {
                totalTimeGiven += question.TimeGiven;
            }
        }


        if (totalTimeGiven.Seconds > 0)
        {
            totalTimeGiven = totalTimeGiven.Add(TimeSpan.FromMinutes(1));
        }


        return new SuccessDataResult<string>(totalTimeGiven.ToString(@"hh\:mm"), Messages.TotalTimeCalculated);
    }
}
