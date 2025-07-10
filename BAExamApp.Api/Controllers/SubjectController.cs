using BAExamApp.Business.ApiServices.Concrete;
using BAExamApp.Business.ApiServices.Interfaces;
using BAExamApp.Business.Constants;
using BAExamApp.Business.Interfaces.Services;
using BAExamApp.Dtos.ApiDtos.ProductSubjectApiDtos;
using BAExamApp.Dtos.ApiDtos.SubjectApiDtos;
using BAExamApp.Entities.DbSets;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BAExamApp.Api.Controllers;
[Route("api/[controller]/[action]")]
[ApiController]
public class SubjectController : ControllerBase
{
    private readonly ISubjectApiService _subjectApiService;
    private readonly IRegisterCodeApiService _registerCodeService;

    public SubjectController(ISubjectApiService subjectService, IRegisterCodeApiService registerCodeService)
    {
        _subjectApiService = subjectService;
        _registerCodeService = registerCodeService;
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromForm] SubjectUpdateRequestApiDto request, string code)
    {
        if (await _registerCodeService.IsRegisterCodeActiveAsync(code))
        {
            try
            {
                if (id != request.Id)
                    return BadRequest(new { Message = Messages.InvalidID });

                if (!ModelState.IsValid)
                    return BadRequest(new { Message = ModelState });

                if (string.IsNullOrWhiteSpace(request.Name))
                    return BadRequest(new { Message = Messages.SubjectNameEmpty });

                var existingSubject = await _subjectApiService.GetByIdAsync(id);

                if (!existingSubject.IsSuccess)
                    return NotFound(new { Message = Messages.SubjectNotFound });

                var updatedProductSubjects = request.ProductIds
                    .Select(p => new ProductSubjectApiDto
                    {
                        ProductId = p,
                        SubjectId = request.Id
                    })
                    .ToList();

                var updateDto = new SubjectUpdateApiDto
                {
                    Id = id,
                    Name = request.Name,
                    Subtopics = existingSubject.Data.Subtopics,
                   // ProductSubjects = updatedProductSubjects
                };

                var result = await _subjectApiService.UpdateAsync(updateDto);
                if (!result.IsSuccess)
                    return BadRequest(new { Message = Messages.UpdateFail });

                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.IgnoreCycles,
                    WriteIndented = true
                };

                var response = new
                {
                    Values = result.Data,
                    Message = Messages.UpdateSuccess,
                };

                return Ok(JsonSerializer.Serialize(response, options));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"An error occurred: {ex.Message}" });
            }
        }

        return NotFound(new { Message = Messages.CodeNotActive });
    }

    [HttpPost("AddAsync")]
    public async Task<IActionResult> AddAsync(SubjectCreateApiDto entity, [FromQuery] string registerCode)
    {

        if (!(await _registerCodeService.IsRegisterCodeActiveAsync(registerCode)))
            return Unauthorized(Messages.AddFail);

        var result = await _subjectApiService.AddAsync(entity);

        if (!result.IsSuccess)
            return Unauthorized(result.Message);

        return Ok(new { Values = result.Data, Message = result.Message });
    }
    /// <summary>
    /// Tüm konuların listelenme işlemi. 
    /// RegisterCode kontrolü yapar olumsuz durumda geri dönüş değerleri belirtilmiştir.
    /// </summary>
    /// <param name="registerCode"></param>
    /// <returns></returns>

    [HttpGet("GetAllSubjects")]
    public async Task<IActionResult> GetAllSubjects(string registerCode)
    {
        bool isCodeActive = await _registerCodeService.IsRegisterCodeActiveAsync(registerCode);
        if (!isCodeActive) return Unauthorized(Messages.InvalidRegisterCode);
        var allSubjects = await _subjectApiService.GetAllAsync();
        if (allSubjects == null) return NotFound(Messages.ListNotFound);
        return Ok(allSubjects);
    }
    /// <summary>
    /// Id'den bulduğu konuyu silme işlemini gerçekleştirir.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="registerCode"></param>
    /// <returns></returns>
    [HttpDelete("DeleteSubject/{id}")]
    public async Task<IActionResult> DeleteSubject(Guid id, string registerCode)
    {
        if (!await _registerCodeService.IsRegisterCodeActiveAsync(registerCode))
        {
            return Unauthorized(new
            {
                Message = Messages.AddUserRoleFail
            });
        }

        try
        {
            var deleteResult = await _subjectApiService.DeleteAsync(id);
            if (!deleteResult.IsSuccess)
            {
                return BadRequest(new
                {
                    Message = Messages.DeleteFail
                });
            }
            return Ok(new
            {
                Message = Messages.DeleteSuccess
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Message = Messages.DeleteFail
            });
        }
    }
}
