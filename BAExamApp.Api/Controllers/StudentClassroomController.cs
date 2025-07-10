using BAExamApp.Business.ApiServices.Concrete;
using BAExamApp.Business.ApiServices.Interfaces;
using BAExamApp.Business.Constants;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BAExamApp.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class StudentClassroomController : ControllerBase
{
    private readonly IRegisterCodeApiService _registerCodeApiService;
    private readonly IStudentClassroomApiService _studentClassroomApiService;

    public StudentClassroomController(IRegisterCodeApiService registerCodeApiService, IStudentClassroomApiService studentClassroomApiService)
    {
        _registerCodeApiService = registerCodeApiService;
        _studentClassroomApiService = studentClassroomApiService;
    }
    /// <summary>
    /// Register kodu kontrol edip doğru olması durumunda öğrenci id, adı ve sınıfının id ve adını listeler
    /// </summary>
    /// <returns></returns>
    [HttpGet("GetAllStudentsWithClassrooms")]
    public async Task<IActionResult> GetAllStudentsWithClassrooms(string registerCode)
    {
        bool isCodeActive = await _registerCodeApiService.IsRegisterCodeActiveAsync(registerCode);
        if (!isCodeActive) return Unauthorized(Messages.InvalidRegisterCode);

        var result = await _studentClassroomApiService.GetAllStudentsWithClassroomAsync();
        if (result == null) return NotFound(Messages.ListNotFound);
        return Ok(result);
    }

}
