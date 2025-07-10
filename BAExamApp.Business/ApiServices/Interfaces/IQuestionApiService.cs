using BAExamApp.Dtos.ApiDtos.QuestionApiDtos;
using BAExamApp.Dtos.Questions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Business.ApiServices.Interfaces;
public interface IQuestionApiService
{
    /// <summary>
    /// sınav kuralında olması istenen özelliklerdeki soruları sorgular ve liste olarak getirir.
    /// </summary>
    /// <param name="questionDifficultyId"> seçilen kuralın içeriğindeki sorunun istenen leveli </param>
    /// <param name="questionType"> seçilen kuralın içeriğindeki sorunun tipi</param>
    /// <param name="subjectId"> seçilen kuralın içeriğindeki subject id'si</param>
    /// <returns> istenen özelliklerdeki soruları listeleyerek döner </returns>
    Task<IDataResult<List<QuestionListApiDto>>> GetAllByExamRuleSubtopicAsync(Guid questionDifficultyId, int questionType, List<Guid> subtopicId);
}
