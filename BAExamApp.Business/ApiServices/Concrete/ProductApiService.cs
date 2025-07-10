using BAExamApp.DataAccess.Interfaces.Repositories;
using BAExamApp.Dtos.ApiDtos.ProductApiDtos;
using BAExamApp.Dtos.Products;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Business.ApiServices.Concrete;
public class ProductApiService : IProductApiService
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly IClassroomProductRepository _classroomProductRepository;
    private readonly IProductSubjectRepository _productSubjectRepository;
    private readonly IExamRuleRepository _examRuleRepository;
    private readonly ITrainerProductRepository _trainerProductRepository;

    public ProductApiService(IProductRepository productRepository, IMapper mapper, IClassroomProductRepository classroomProductRepository, IProductSubjectRepository productSubjectRepository, IExamRuleRepository examRuleRepository, ITrainerProductRepository trainerProductRepository)
    {
        _productRepository = productRepository;
        _mapper = mapper;
        _classroomProductRepository = classroomProductRepository;
        _productSubjectRepository = productSubjectRepository;
        _examRuleRepository = examRuleRepository;
        _trainerProductRepository = trainerProductRepository;
    }

    public async Task<IResult> DeleteAsync(Guid id)
    {
        var product = await _productRepository.GetByIdAsync(id);

        if (product is null)
        {
            return new ErrorDataResult<ProductApiDto>(Messages.ProductNotFound);
        }

        var classroomUsingProduct = await IsClassroomUsedProductAsync(product.Id);
        var trainerUsingProduct = await IsTrainerUsedProductAsync(product.Id);
        var examRuleUsingProduct = await IsExamRuleUsedProductAsync(product.Id);

        if (classroomUsingProduct || trainerUsingProduct || examRuleUsingProduct)
        {
            product.Status = Core.Enums.Status.Passive;
            await _productRepository.SaveChangesAsync();
            return new SuccessResult(Messages.SetIsActiveFalse);
        }

        await _productRepository.DeleteAsync(product);
        await _productRepository.SaveChangesAsync();

        return new SuccessResult(Messages.DeleteSuccess);
    }

    public async Task<bool> IsClassroomUsedProductAsync(Guid productId)
    {
        var classroomUsingProduct = await _classroomProductRepository.AnyAsync(e => e.ProductId == productId);
        return classroomUsingProduct;
    }
    public async Task<bool> IsTrainerUsedProductAsync(Guid productId)
    {
        var trainerUsingProduct = await _trainerProductRepository.AnyAsync(e => e.ProductId == productId);
        return trainerUsingProduct;
    }
    public async Task<bool> IsExamRuleUsedProductAsync(Guid productId)
    {
        var examRuleUsingProduct = await _examRuleRepository.AnyAsync(e => e.ProductId == productId);
        return examRuleUsingProduct;
    }

    /// <summary>
    /// Verilen <see cref="ProductUpdateDto"/> kullanılarak mevcut bir ürünün bilgilerini günceller.
    /// </summary>
    /// <param name="productUpdateDto">
    /// Güncellenmesi istenen ürün bilgilerini içeren DTO. Ürün ID'si, adı, durumu, aktiflik bilgisi 
    /// ve ilişkili alt ürün bilgilerini (ProductSubjects) içerir. <c>ProductSubjects</c> alanı 
    /// boş veya null olabilir.
    /// </param>
    /// <returns>
    /// Güncelleme başarılı olursa, güncellenmiş ürün bilgilerini içeren bir 
    /// <see cref="SuccessDataResult{T}"/> döner. 
    /// Eğer ürün bulunamazsa veya aynı isimde başka bir ürün mevcutsa, hata mesajı içeren bir 
    /// <see cref="ErrorDataResult{T}"/> döner.
    /// <returns>
    public async Task<IDataResult<ProductDto>> UpdateAsync(ProductUpdateDto productUpdateDto)
    {
        try
        {
            var product = await _productRepository.GetByIdAsync(productUpdateDto.Id);
            if (product == null)
            {
                return new ErrorDataResult<ProductDto>(Messages.ProductNotFound);
            }

            var hasDuplicateName = await _productRepository.AnyAsync(p =>
                p.Name.ToLower() == productUpdateDto.Name.Trim().ToLower() && p.Id != productUpdateDto.Id);
            if (hasDuplicateName)
            {
                return new ErrorDataResult<ProductDto>(Messages.AddFailAlreadyExists);
            }

            if (productUpdateDto.ProductSubjects != null && productUpdateDto.ProductSubjects.Any())
            {
                foreach (var productSubjectDto in productUpdateDto.ProductSubjects)
                {
                    var existingProductSubject = product.ProductSubjects
                        ?.FirstOrDefault(ps => ps.Id == productSubjectDto.Id);

                    if (existingProductSubject != null)
                    {
                        if (existingProductSubject.ProductId != productSubjectDto.ProductId)
                        {
                            product.ProductSubjects.Remove(existingProductSubject);
                            await _productRepository.SaveChangesAsync();

                            product.ProductSubjects.Add(new ProductSubject
                            {
                                ProductId = productSubjectDto.ProductId,
                            });
                        }
                    }
                }
            }

            product.Name = productUpdateDto.Name.Trim();
            product.IsActive = productUpdateDto.IsActive;
            product.Status = productUpdateDto.Status;

            await _productRepository.UpdateAsync(product);
            await _productRepository.SaveChangesAsync();
        
            var updatedProductDto = _mapper.Map<ProductDto>(product);
            return new SuccessDataResult<ProductDto>(updatedProductDto, Messages.UpdateSuccess);
        }
        catch (Exception ex)
        {
            return new ErrorDataResult<ProductDto>($"{Messages.UpdateFail}: {ex.Message}");
        }
    }

    /// <summary>
    /// Product ekleme işlemini yapar.
    /// Product'ın Name, isActive, TechnicalUnitId propertylerini alır _productRepository ile Db'ye ekler ve kaydeder. 
    /// </summary>
    /// <returns>ProductDTO döndürür</returns> 
    public async Task<IDataResult<ProductDto>> AddAsync(ProductCreateDto productCreateDto)
    {
        var hasProduct = await _productRepository.AnyAsync(product => product.Name.ToLower() == productCreateDto.Name.Trim().ToLower());

        if (hasProduct)
            return new ErrorDataResult<ProductDto>(Messages.AddFailAlreadyExists);

        var product = _mapper.Map<Product>(productCreateDto);

        await _productRepository.AddAsync(product);
        await _productRepository.SaveChangesAsync();

        return new SuccessDataResult<ProductDto>(_mapper.Map<ProductDto>(product), Messages.AddSuccess);
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync()
    {
        var allProducts = await _productRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<ProductDto>>(allProducts); 


    }

   
}
