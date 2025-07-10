using BAExamApp.Dtos.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Business.ApiServices.Interfaces;
public interface IProductApiService
{
    
    /// <summary>
    /// Silinmesi gereken ürünleri birbirleriyle bağlantılı oldukları için pasif statüsüne alır.
    /// </summary>
    /// <param name="id">Id'ye göre Product getirir.</param>
    /// <returns></returns>
    Task<IResult> DeleteAsync(Guid id);

    /// <summary>
    /// Ürünü herhangi bir sınıf kullanıyor mu kontrol eder.
    /// </summary>
    /// <param name="productId">ProductId'ye göre product getirir.</param>
    /// <returns></returns>
    Task<bool> IsClassroomUsedProductAsync(Guid productId);
    /// <summary>
    /// Ürünü herhangi bir trainer kullanıyor mu kontrol eder
    /// </summary>
    /// <param name="productId">ProductId'ye göre product getirir.</param>
    /// <returns></returns>
    Task<bool> IsTrainerUsedProductAsync(Guid productId);
    /// <summary>
    /// Herhangi bir sınav kuralında kullanılıyomu kontrol eder.
    /// </summary>
    /// <param name="productId">ProductId'ye göre product getirir.</param>
    /// <returns></returns>
    Task<bool> IsExamRuleUsedProductAsync(Guid productId);

    /// <summary>
    /// Belirtilen ProductUpdateDto kullanılarak mevcut bir ürünü günceller.
    /// </summary>
    /// <param name="productUpdateDto">
    /// Güncellenmesi istenen ürün bilgilerini içeren DTO. Güncellenmesi gereken ürünün ID'si, adı, durumu, aktiflik bilgisi ve ilişkili alt ürün bilgilerini içerir.
    /// </param>
    /// <returns>
    Task<IDataResult<ProductDto>> UpdateAsync(ProductUpdateDto productUpdateDto);


    /// <summary>
    /// Product ekleme işlemini yapar.
    /// Product'ın Name, isActive, TechnicalUnitId propertylerini alır _productRepository ile Db'ye ekler ve kaydeder. 
    /// </summary>
    /// <returns>ProductDTO döndürür</returns>
    Task<IDataResult<ProductDto>> AddAsync(ProductCreateDto productCreateDto);
    /// <summary>
    /// Product listeleme işlemini yapar.
    /// Product'ın Name, isActive, TechnicalUnitId propertylerini alır _productRepository ile Db'den getirir 
    /// </summary>
    /// <returns>IEnumerable<ProductDto>> döndürür</returns>
    Task<IEnumerable<ProductDto>> GetAllProductsAsync();


}
