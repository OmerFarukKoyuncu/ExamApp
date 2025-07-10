using BAExamApp.Business.ApiServices.Concrete;
using BAExamApp.Business.ApiServices.Interfaces;
using BAExamApp.Business.Constants;
using BAExamApp.Dtos.ApiDtos.GroupTypeApiDtos;
using BAExamApp.Dtos.GroupTypes;
using BAExamApp.Dtos.Products;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BAExamApp.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class GroupTypeController : ControllerBase
{
    private readonly IGroupTypeApiService _groupTypeApiService;
    private readonly IRegisterCodeApiService _registerCodeApiService;

    public GroupTypeController(IGroupTypeApiService groupTypeApiService, IRegisterCodeApiService registerCodeApiService)
    {
        _groupTypeApiService = groupTypeApiService;
        _registerCodeApiService = registerCodeApiService;
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, string code)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(code) || !await _registerCodeApiService.IsRegisterCodeActiveAsync(code))
            {
                return NotFound(new { Message = Messages.CodeNotActive });
            }

            var result = await _groupTypeApiService.DeleteAsync(id);

            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok(result);
        }
        catch (Exception ex)
        {

            // Log the exception here
            return StatusCode(500, new { Message = Messages.UpdateFail + ex.Message });
        }
    }

    [HttpGet("GetAllGroupTypes")]
    public async Task<IActionResult> GetAllGroupTypes(string registerCode)
    {
        bool isCodeActive = await _registerCodeApiService.IsRegisterCodeActiveAsync(registerCode);
        if (!isCodeActive) return Unauthorized(Messages.InvalidRegisterCode);
        var allGroupTypes= await _groupTypeApiService.GetAllAsync();
        if (allGroupTypes == null) return NotFound(Messages.ListNotFound);
        return Ok(allGroupTypes);
    }

    /// <summary>
    /// Verilen GroupTypeUpdateApiDto ve registerCode ile grup türünü günceller.
    /// Register kodu aktif değilse Unauthorized döner.
    /// Güncelleme başarısız olursa BadRequest, başarılı olursa güncellenmiş verilerle birlikte Ok döner.
    /// </summary>

    [HttpPut("Update")]
    public async Task<IActionResult> UpdateAsync([FromBody] GroupTypeUpdateApiDto groupTypeUpdate, [FromQuery] string registerCode)
    {
        if (!(await _registerCodeApiService.IsRegisterCodeActiveAsync(registerCode)))
            return Unauthorized(Messages.AddUserRoleFail);

        var updateResult = await _groupTypeApiService.UpdateAsync(groupTypeUpdate);

        if (!updateResult.IsSuccess)
            return BadRequest(updateResult.Message);

        return Ok(new
        {
            Values = updateResult.Data,
            Message = updateResult.Message
        });
    }

    [HttpPost("AddGroupType")]
    public async Task<IActionResult> AddGroupType(string registerCode, GroupTypeCreateApiDto groupTypeCreateApiDto)
    {
        if (!await _registerCodeApiService.IsRegisterCodeActiveAsync(registerCode))
        {
            return Unauthorized(new
            {
                Message = Messages.AddUserRoleFail
            });
        }

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(x => x.Errors)
                                          .Select(e => e.ErrorMessage)
                                          .Distinct();

            return BadRequest(new
            {
                Message = Messages.AddError
            });
        }

        try
        {
            var addResult = await _groupTypeApiService.AddAsync(groupTypeCreateApiDto);

            if (!addResult.IsSuccess)
            {
                return BadRequest(new
                {
                    Message = Messages.AddError
                });
            }

            return Ok(new
            {
                Message = Messages.AddSuccess
            });
        }
        catch (Exception)
        {
            return BadRequest(Messages.AddError);
        }
    }
}
