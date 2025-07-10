using BAExamApp.Business.ApiServices.Concrete;
using BAExamApp.Business.ApiServices.Interfaces;
using BAExamApp.Business.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BAExamApp.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ClassroomController : ControllerBase
{
    private readonly IRegisterCodeApiService _registerCodeApiService;
    private readonly IClassroomApiService _classroomApiService;

    public ClassroomController(IRegisterCodeApiService registerCodeApiService, IClassroomApiService classroomApiService)
    {
        _registerCodeApiService = registerCodeApiService;
        _classroomApiService = classroomApiService;
    }


    /// <summary>
    /// Girilen register code değeri doğru ise bütün sınıfları gösterir.
    /// </summary>
    /// <param name="registerCode">Login olduğumuzda gelen doğrulama kodu</param>
    /// <returns></returns>
    [HttpGet("GetAllClassrooms")]
    public async Task<IActionResult> GetAllClassrooms(string registerCode)
    {
        bool isCodeActive = await _registerCodeApiService.IsRegisterCodeActiveAsync(registerCode);
        if (!isCodeActive) return Unauthorized(Messages.InvalidRegisterCode);

        var allClassrooms = await _classroomApiService.GetAllAsync();
        if (allClassrooms == null) return NotFound(Messages.ListNotFound);
        return Ok(allClassrooms);
    }
}
