using BAExamApp.Business.ApiServices.Interfaces;
using BAExamApp.Business.Constants;
using BAExamApp.Business.Interfaces.Services;
using BAExamApp.Business.Services;
using BAExamApp.Dtos.Exams;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BAExamApp.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ExamsController : ControllerBase
{
    private readonly IExamApiService _examApiService;
    private readonly IRegisterCodeApiService _registerCodeApiService;
    private readonly IExamRuleApiService _examRuleService;
    private readonly IStudentQuestionApiService _studentQuestionService;

    public ExamsController(IExamApiService examApiService, IRegisterCodeApiService registerCodeApiService, IExamRuleApiService examRuleService, IStudentQuestionApiService studentQuestionService)
    {
        _examApiService = examApiService;
        _registerCodeApiService = registerCodeApiService;
        _examRuleService = examRuleService;
        _studentQuestionService = studentQuestionService;
    }

    [HttpGet("GetAllData")]
    public async Task<IActionResult> GetAllData(string registerCode)
    {
        if (await _registerCodeApiService.IsRegisterCodeActiveAsync(registerCode))
        {

            var values = await _examApiService.GetAllDataWithRegisterCodeAsync();
            return Ok(new { Values = values.Data, Message = values.Message });
        }
        else return Unauthorized("Kullanma izniniz yok");
    }

    [HttpGet("GetTotalTimeByRule")]
    public async Task<IActionResult> GetTotalTimeByRule(Guid examRuleId, string registerCode)
    {
        if (await _registerCodeApiService.IsRegisterCodeActiveAsync(registerCode))
        {
            try
            {
                var examRule = await _examRuleService.GetByIdAsync(examRuleId);
                var totalGivenTimeResult = await _studentQuestionService.TotalGivenTimeAsync(examRule.Data.ExamRuleSubtopics);

                if (totalGivenTimeResult.IsSuccess)
                {
                    return Ok(new { totalTime = totalGivenTimeResult.Data });
                }

                return Unauthorized(totalGivenTimeResult.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }

        }
        else
        {
            return Unauthorized(Messages.AddFail);
        }
    }
    /// <summary>
    /// Sistemde mevcut olan sınav türlerini döndürür.
    /// </summary>
    /// <param name="registerCode">
    /// Kullanıcının erişimini doğrulamak için kullanılan kayıt kodu.
    /// Kayıt kodunun aktif olması gerekmektedir.
    /// </param>
    /// <returns>
    /// Eğer kayıt kodu geçerli ve aktifse sınav türlerinin bir listesini döndürür.
    /// - Kayıt kodu geçersiz veya aktif değilse 401 Unauthorized yanıtı döner.
    /// - Sınav türlerini getirirken bir sorun oluşursa 400 Bad Request yanıtı döner.
    /// </returns>
    /// <remarks>
    /// Bu endpoint, istemcinin sistemdeki mevcut sınav türlerini görüntüleyebilmesi 
    /// ve diğer işlemlerde kullanılabilmesi için tasarlanmıştır.
    /// </remarks>

    [HttpGet("GetExamTypes")]
    public async Task<IActionResult> GetExamTypes([FromQuery] string registerCode)
    {
        if (!(await _registerCodeApiService.IsRegisterCodeActiveAsync(registerCode)))
            return Unauthorized(Messages.AddUserRoleFail);

        var result = await _examApiService.GetExamTypesAsync();

        if (!result.IsSuccess)
            return BadRequest(result.Message);

        return Ok(new
        {
            Values = result.Data,
            Message = result.Message
        });
    }

    /// <summary>
    /// Kayıt kodunun geçerli olup olmadığını kontrol eder ve geçerli ise 
    /// sınav oluşturma türlerini döndürür.
    /// </summary>
    /// <param name="registerCode">Kullanıcının gönderdiği kayıt kodu.</param>
    /// <returns>
    /// - Geçerli bir kayıt kodu sağlanmışsa:
    ///   - Başarılı sonuç: `Ok` (Sınav oluşturma türleri ve başarı mesajını içerir).
    ///   - Başarısız sonuç: `BadRequest` (Hata mesajını içerir).
    /// - Geçersiz bir kayıt kodu sağlanmışsa: `Unauthorized` (Yetkisiz erişim mesajı ile döner).
    /// </returns>
    [HttpGet("GetExamCreateTypes")]
    public async Task<IActionResult> GetExamCreateTypes(string registerCode)
    {
        if (await _registerCodeApiService.IsRegisterCodeActiveAsync(registerCode))
        {
            var result = await _examApiService.GetExamCreateTypesAsync();
            if (result.IsSuccess)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
        else return Unauthorized(Messages.AddUserRoleFail);

    }

    /// <summary>
    /// Yeni bir sınav oluşturur.
    /// </summary>
    /// <param name="examCreateDto">Oluşturulacak sınav bilgilerini içeren DTO.</param>
    /// <param name="registerCode">Kullanıcının erişimini doğrulamak için kullanılan kayıt kodu.</param>
    /// <returns>
    /// - Başarılı durumda: `Ok` (Oluşturulan sınav bilgileri ve başarı mesajını içerir).
    /// - Hatalı durumda: `BadRequest` veya `Unauthorized` yanıtı döner.
    /// </returns>
    [HttpPost("CreateExam")]
    public async Task<IActionResult> CreateExam([FromBody] ExamCreateDto examCreateDto, [FromQuery] string registerCode)
    {
        if (!(await _registerCodeApiService.IsRegisterCodeActiveAsync(registerCode)))
        {
            return Unauthorized(Messages.AddUserRoleFail);
        }

        var result = await _examApiService.CreateExamAsync(examCreateDto);

        if (!result.IsSuccess)
        {
            return BadRequest(result.Message);
        }

        return Ok(new
        {
            Values = result.Data,
            Message = result.Message
        });
    }

    /// <summary>
    /// Sınav başlatma işlemi yapar
    /// </summary>
    /// <param name="examId">Başlatılacak sınavın GUID ID'si</param>
    /// <param name="registerCode">RegisterCode</param>
    /// <returns>
    /// Eğer RegisterCode aktif değilse Unauthorized(Messages.InvalidRegisterCode) Döner /
    /// Sınav başlatılamadıysa BadRequest(Messages.ExamStartedMailError) Döner /
    /// Sınav başarıyla başlatıldıysa Ok(Messages.ExamStartedSuccessfully) Döner /
    /// </returns>
    [HttpPost("StartExam")]
    public async Task<IActionResult> StartExam(Guid examId, string registerCode)
    {
        bool result = await _registerCodeApiService.IsRegisterCodeActiveAsync(registerCode);
        if(!result) return Unauthorized(Messages.InvalidRegisterCode);        
        string link = Url.Action("StartExam", "exam", new { Area = "Student" }, Request.Scheme);
        var examResult = await _examApiService.StartExam(examId, link);
        if(!examResult.IsSuccess) return BadRequest(Messages.ExamStartedMailError);
        return Ok(Messages.ExamStartedSuccessfully);
    }

    /// <summary>
    /// İlgili sınıf için sınava giren öğrencilerin sonuçlarını döndürür
    /// Başarılı > IEnumerable<StudentExamListDto></StudentExamListDto> döner
    /// Başarısız > StudentExam_Found_Success Mesajı döner
    /// </summary>
    /// <param name="examId">Sonuçları gösterilecek sınavın GUID ID'si</param>
    /// <param name="registerCode">RegisterCode</param>
    /// <returns></returns>
    [HttpGet("GetStudentsExamResult")]
    public async Task<IActionResult> GetStudentsExamResult(Guid examId, string registerCode)
    {
        bool result = await _registerCodeApiService.IsRegisterCodeActiveAsync(registerCode);
        if (!result) return Unauthorized(Messages.InvalidRegisterCode);
        var examResult = await _examApiService.GetExamResultForStudentsByExamId(examId);
        if (!examResult.IsSuccess) return BadRequest(examResult.Message);
        return Ok(examResult);
    }
}
