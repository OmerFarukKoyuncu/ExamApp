using BAExamApp.Dtos.ApiDtos.ExamRuleApiDtos;
using BAExamApp.Dtos.ExamRules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Business.ApiServices.Interfaces;
public interface IExamRuleApiService
{
    /// <summary>
    /// Id parametresine göre sınav kuralını getirir.
    /// </summary>
    /// <param name="id">Seçilen ExamRule id sine ait sınav kuralını getirir</param>
    /// <returns>Eğer id parametresine sahip bir sınav kuralı yoksa notfound mesajı döner, bulunursa da success mesajı döner</returns>
    Task<IDataResult<ExamRuleApiDto>> GetByIdAsync(Guid id);
    /// <summary>
    /// Sınav kuralarını listeler
    /// </summary>
    /// <returns></returns>
    public Task<IDataResult<List<ExamRuleApiDto>>> GetAllExamRule();
}
