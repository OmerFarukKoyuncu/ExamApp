using BAExamApp.Dtos.ApiDtos.ExamRuleSubtopicApiDtos;
using BAExamApp.Dtos.ApiDtos.QuestionApiDtos;
using BAExamApp.Dtos.ExamRuleSubtopics;
using BAExamApp.Dtos.Questions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Business.ApiServices.Interfaces;
public interface IStudentQuestionApiService
{
    /// <summary>
    /// Gönderilen sınav kurallarına göre genel soru havuzundan istenen soru seviyesi ve soru tipine göre istenen miktarda rastegele soru seçilerek bir soru listesi oluşturur.
    /// </summary>
    /// <param name="examRuleSubtopic">ExamRuleSubtopic</param>
    /// <returns>DataResult<QuestionForStudentListDto></returns>
    Task<IDataResult<List<QuestionListApiDto>>> CreateQuestionPoolForExamRuleSubtopicsAsync(List<ExamRuleSubtopicApiDto> examRuleSubtopics);
    /// <summary>
    /// Gönderilen alt konuların sorularının toplam sürelerini döndürür.
    /// </summary>
    /// <param name="subtopics"></param>
    /// <returns></returns>
    Task<IDataResult<string>> TotalGivenTimeAsync(IEnumerable<ExamRuleSubtopicApiDto> subtopics);
}
