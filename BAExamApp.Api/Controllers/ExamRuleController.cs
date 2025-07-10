using BAExamApp.Business.ApiServices.Concrete;
using BAExamApp.Business.ApiServices.Interfaces;
using BAExamApp.Business.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BAExamApp.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ExamRuleController : ControllerBase
{
    private readonly IRegisterCodeApiService _registerCodeApiService;
    private readonly IExamRuleApiService _examRuleApiService;
 

    public ExamRuleController(IRegisterCodeApiService registerCodeApiService,IExamRuleApiService examRuleApiService)
    {
        _registerCodeApiService = registerCodeApiService;
        _examRuleApiService = examRuleApiService;
        
    }
    [HttpGet("GetAllExamRule")]
    public async Task<IActionResult> GetAllExamRule(string registerCode)
    {
        bool isCodeActive = await _registerCodeApiService.IsRegisterCodeActiveAsync(registerCode);
        if (!isCodeActive) return Unauthorized(Messages.InvalidRegisterCode);

        var result = await _examRuleApiService.GetAllExamRule(); 

        if (result == null) return NotFound(Messages.ListNotFound);
        return Ok(result);
    }
}
