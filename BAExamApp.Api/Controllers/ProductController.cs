using BAExamApp.Business.ApiServices.Concrete;
using BAExamApp.Business.ApiServices.Interfaces;
using BAExamApp.Business.Constants;
using BAExamApp.Business.Interfaces.Services;
using BAExamApp.Business.Services;
using BAExamApp.Dtos.GroupTypes;
using BAExamApp.Dtos.Products;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BAExamApp.Api.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IRegisterCodeApiService _registerCodeApiService;
    private readonly IProductApiService _productService;

    public ProductController(IRegisterCodeApiService registerCodeApiService,IProductApiService productService)
    {
        _registerCodeApiService = registerCodeApiService;
        _productService = productService;
    }


    
    /// <summary>
    /// Id'den çektiğimiz product nesnesini ilk başta registerCode kontrolü yapar ve sonrasında delete işlemi yapar.
    /// </summary>
    /// <param name="registerCode"></param>
    /// <param name="productId"></param>
    /// <returns></returns>
    [HttpDelete]
    public async Task<IActionResult> DeleteProduct(string registerCode, Guid productId)
    {
        if (!await _registerCodeApiService.IsRegisterCodeActiveAsync(registerCode))
        {
            return Unauthorized(new
            {
                Details = Messages.AddUserRoleFail         
            });
        }
       
        try
        {
            var deleteProduct = await _productService.DeleteAsync(productId); ;
            if (!deleteProduct.IsSuccess)
            {
                return BadRequest(new
                {
                    Message = Messages.DeleteFail,
                    Details = deleteProduct.Message
                });
            }
            return Ok(new
            {
                Message = Messages.DeleteSuccess,
                Details = deleteProduct.Message
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

    /// <summary>
    /// Verilen ProductUpdateDto kullanılarak bir ürünü günceller ve sonuç mesajıyla birlikte döndürür.
    /// </summary>
    /// <param name="productUpdateDto">
    /// Güncellenmesi istenen ürün bilgilerini içeren DTO. Ürün ID'si, adı, durumu, aktiflik bilgisi ve ilişkili alt ürün bilgilerini içerir.
    /// </param>
    /// <param name="registerCode">
    /// Ürünü güncelleme işlemini gerçekleştirmek için gerekli olan kayıt kodu. Kayıt kodunun aktif olması kontrol edilir.
    /// </param>
    /// <returns>
    /// Eğer kayıt kodu geçersizse, Unauthorized sonucu döner.
    /// Güncelleme işlemi başarısız olursa, BadRequest sonucu döner ve hata mesajını içerir.
    /// Güncelleme başarılı olursa, güncellenmiş ürün bilgilerini ve başarı mesajını içeren bir Ok sonucu döner.
    /// </returns>
    [HttpPut("Update")]
    public async Task<IActionResult> UpdateAsync([FromBody] ProductUpdateDto productUpdateDto, [FromQuery] string registerCode)
    {
        if (!(await _registerCodeApiService.IsRegisterCodeActiveAsync(registerCode)))
            return Unauthorized(Messages.AddUserRoleFail);

        var updateResult = await _productService.UpdateAsync(productUpdateDto);

        if (!updateResult.IsSuccess)
            return BadRequest(updateResult.Message);

        return Ok(new
        {
            Values = updateResult.Data,
            Message = updateResult.Message
        });
    }


    /// <summary>
    /// Registor code parametresi ile kimlik doğrulaması yapılır.
    /// Kimlik doğrulamasından sonra ProductCreateDTO alınarak product ekleme işlemi gerçekleşir
    /// </summary>
    /// <param name="registerCode"></param>
    /// <param name="productCreateDto"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> AddAsync(string registerCode, ProductCreateDto productCreateDto)
    {
        if (await _registerCodeApiService.IsRegisterCodeActiveAsync(registerCode))
            return Ok(await _productService.AddAsync(productCreateDto));

        return Unauthorized(new { Message = Messages.AddUserRoleFail });
    }

    /// <summary>
    /// Getirilen tüm ürünleri döndüren bir API metodudur.
    /// </summary>
    /// <returns>
    /// Eğer ürünler varsa, HTTP 200 (OK) ile ürün listesini döner.
    /// Eğer ürün bulunamazsa, HTTP 404 (Not Found) ile "Hiç ürün bulunamadı" mesajı döner.

    [HttpGet("List")]
    public async Task<IActionResult> GetAllProductAsync()
    {
        var products = await _productService.GetAllProductsAsync(); 
        return products != null || products.Count()!=0 && products.Any() ? Ok(products) : NotFound("Hiç ürün bulunamadı.");
    }



}
