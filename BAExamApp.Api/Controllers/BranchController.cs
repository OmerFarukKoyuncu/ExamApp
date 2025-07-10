using AutoMapper;
using BAExamApp.Business.ApiServices.Interfaces;
using BAExamApp.Business.Constants;
using BAExamApp.Dtos.ApiDtos.BranchApiDtos;
using BAExamApp.Dtos.ApiUsers;
using BAExamApp.Entities.DbSets;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BAExamApp.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class BranchController : ControllerBase
{
    private readonly IRegisterCodeApiService _registerCodeApiService;
    private readonly IBranchApiService _branchApiService;

    public BranchController(IRegisterCodeApiService registerCodeApiService, IBranchApiService branchApiService, IMapper mapper)
    {
        _registerCodeApiService = registerCodeApiService;
        _branchApiService = branchApiService;
    }



    /// <summary>
    /// Id'den çektiğimiz branch nesnesini ilk başta registerCode kontrolü yapar ve sonrasında delete işlemi yapar.
    /// </summary>
    /// <param name="registerCode"></param>
    /// <param name="branchId"></param>
    /// <returns></returns>
    [HttpDelete]
    public async Task<IActionResult> DeleteBranch(string registerCode, Guid branchId)
    {
        if (!await _registerCodeApiService.IsRegisterCodeActiveAsync(registerCode))
        {
            return Unauthorized(new
            {
                Details = Messages.CodeNotActive,
            });
        }

        try
        {
            var deleteBranch = await _branchApiService.DeleteAsync(branchId); ;
            if (!deleteBranch.IsSuccess)
            {
                return BadRequest(new
                {
                    Message = Messages.DeleteFail,
                    Details = deleteBranch.Message
                });
            }
            return Ok(new
            {
                Message = Messages.DeleteSuccess,
                Details = deleteBranch.Message
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Message = Messages.DeleteFail,
                Details = ex.Message
            });
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddAsync(BranchCreateApiDto branchCreateApiDto, [FromQuery] string registerCode)
    {
        if (!await _registerCodeApiService.IsRegisterCodeActiveAsync(registerCode))
        {
            return Unauthorized(new
            {
                Details = Messages.CodeNotActive,
            });
        }

        var result = await _branchApiService.AddAsync(branchCreateApiDto);

        if(!result.IsSuccess)
        {
            return Unauthorized(new
            {
                Details = Messages.AddFail,
            });
        }

        return Ok(new { Data = result.Data, Message = result.Message });

    }

    /// <summary>
    /// RegisterCode aktif durumda ise bütün şubelerin listelenmesi işlemi gerçekleşir.
    /// Şube mevcut değilse uygun yanıt döner.
    /// </summary>
    /// <param name="registerCode"></param>
    /// <returns></returns>
    [HttpGet("GetAllBranches")]
    public async Task<IActionResult> GetAllBranches(string registerCode)
    {
        bool isCodeActive = await _registerCodeApiService.IsRegisterCodeActiveAsync(registerCode);
        if (!isCodeActive) return Unauthorized(Messages.InvalidRegisterCode);

        var allBranches = await _branchApiService.GetAllAsync();
        if (allBranches == null) return NotFound(Messages.ListNotFound);
        return Ok(allBranches);
    }

    /// <summary>
    /// Register Kod doğru ise ve ID eşleşmesi sağlarsa mevcut bir Şube'nin bilgilerini günceller.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="branchApiUpdateDTO"></param>
    /// <param name="registerCode"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromForm] BranchApiUpdateDto branchApiUpdateDTO, string registerCode)
    {
        if (!(await _registerCodeApiService.IsRegisterCodeActiveAsync(registerCode)))
            return Unauthorized(Messages.AddUserRoleFail);

        // ID eşleşmesini kontrol et
        if (id != branchApiUpdateDTO.Id)
        {
            return BadRequest("ID eşleşmiyor.");
        }

        // Güncelleme işlemini gerçekleştir
        var result = await _branchApiService.UpdateAsync(branchApiUpdateDTO);

        if (!result.IsSuccess)
        {
            // Hata durumunda uygun yanıt döndür
            if (result.Message == Messages.BranchNotFound)
            {
                return NotFound(result.Message);
            }

            return BadRequest(result.Message);
        }

        // Başarılı güncelleme
        return Ok(result.Data);
    }

}
