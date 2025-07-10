using BAExamApp.Business.ApiServices.Interfaces;
using BAExamApp.Business.Constants;
using BAExamApp.Business.Interfaces.Services;
using BAExamApp.Business.Services;
using BAExamApp.Dtos.GroupTypes;
using BAExamApp.Entities.DbSets;
using Microsoft.AspNetCore.Http;
using BAExamApp.Dtos.ApiDtos.SubtopicApiDtos;
using BAExamApp.Dtos.Subtopics;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;

namespace BAExamApp.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class SubtopicController : ControllerBase
{
    private readonly IRegisterCodeApiService _registerCodeApiService;
    private readonly ISubtopicApiService _subtopicApiService;

     

    public SubtopicController(ISubtopicApiService subtopicApiService, IRegisterCodeApiService registerCodeApiService)
    {
        _registerCodeApiService = registerCodeApiService;
        _subtopicApiService = subtopicApiService;

    }

    [HttpPost("CreateSubtopic")]
    public async Task<IActionResult> CreateSubtopic(SubtopicCreateApiDto subtopicCreateDto, string registerCode)
    {
        if (await _registerCodeApiService.IsRegisterCodeActiveAsync(registerCode))
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrEmpty(subtopicCreateDto.Name))
            {
                return Unauthorized(Messages.SubtopicNameEmpty);
            }
            else
            {
                subtopicCreateDto.Name =  char.ToUpper(subtopicCreateDto.Name[0]) + subtopicCreateDto.Name.Substring(1).ToLower();
            }

            try
            {
                var createSubtopicResult = await _subtopicApiService.AddAsync(subtopicCreateDto);

                if (!createSubtopicResult.IsSuccess)
                {
                    return Unauthorized(createSubtopicResult.Message);
                }

                return Ok(createSubtopicResult.Data);
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



    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromForm] SubtopicUpdateApiDto request, string code)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(code) || !await _registerCodeApiService.IsRegisterCodeActiveAsync(code))
        {
                return NotFound(new { Message = Messages.CodeNotActive });
            }

            if (id != request.Id)
        {
                return BadRequest(new { Message = Messages.InvalidID });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = Messages.UpdateFail, Errors = ModelState });
        }

            if (string.IsNullOrWhiteSpace(request.Name) && request.SubjectId == null && request.IsActive == null)
        {
                return BadRequest(new { Message = Messages.NoDataToUpdate });
            }

            if (request.IsActive is not null)
            {
                var statusChangeResult = await _subtopicApiService.ChangeRuleStatusAsync(request.Id);
            }
            var updateResult = await _subtopicApiService.UpdateAsync(request);

            if (!updateResult.IsSuccess)
                {
                return NotFound(new { Message = Messages.UpdateFail });
            }

            return Ok(new
            {
                Data = updateResult.Data,
                Message = Messages.UpdateSuccess
            });
        }
        catch (Exception ex)
        {
            // Log the exception here
            return StatusCode(500, new { Message = Messages.UpdateFail + ex.Message });
        }
    }
    /// <summary>
    /// RegisterCode'un aktifliği kontrol edilir. 
    /// RegisterCode aktifse tüm alt konular asenkron alınır. 
    /// Alt konu bulunmazsa uygun yanıt döner. 
    /// </summary>
    /// <param name="registerCode"></param>
    /// <returns></returns>
    [HttpGet("GetAllSubtopics")]
    public async Task<IActionResult> GetAllSubtopics(string registerCode)
    {

        bool isCodeActive = await _registerCodeApiService.IsRegisterCodeActiveAsync(registerCode);
        if (!isCodeActive) return Unauthorized("Geçersiz registercode!");
        var allSubjects = await _subtopicApiService.GetAllAsync();
        if (allSubjects == null) return NotFound("Henüz alt konu bulunmuyor!");
        return Ok(allSubjects);
        

    }
    /// <summary>
    /// Id'den bulduğu altkonuyu silme işlemini gerçekleştirir.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="registerCode"></param>
    /// <returns></returns>
    [HttpDelete("DeleteSubtopic/{id}")]
    public async Task<IActionResult> DeleteSubtopic(Guid id, string registerCode)
    {
        if (!await _registerCodeApiService.IsRegisterCodeActiveAsync(registerCode))
        {
            return Unauthorized(new
            {
                Message = Messages.AddUserRoleFail
            });
        }

        try
        {
            var deleteResult = await _subtopicApiService.DeleteAsync(id);
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

