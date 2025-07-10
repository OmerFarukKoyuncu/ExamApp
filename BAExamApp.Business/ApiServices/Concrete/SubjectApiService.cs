using BAExamApp.DataAccess.Interfaces.Repositories;
using BAExamApp.Dtos.Subjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAExamApp.Core.Enums;
using BAExamApp.Dtos.ApiDtos.SubjectApiDtos;

using System.Collections.Generic;
using System.Threading.Tasks;
using BAExamApp.Entities.DbSets;

namespace BAExamApp.Business.ApiServices.Concrete;
public class SubjectApiService : ISubjectApiService
{
    private readonly ISubjectRepository _subjectRepository;
    private readonly IProductSubjectRepository _productsSubjectsRepository;
    private readonly IMapper _mapper;
    private readonly IQuestionRepository _questionRepository;
    private readonly ISubtopicRepository _subtopicRepository;
    private readonly IProductSubjectRepository _productSubjectRepository;
    private readonly IProductRepository _productRepository;


    public SubjectApiService(
        ISubjectRepository subjectRepository, IProductRepository productRepository, IMapper mapper, IProductSubjectRepository productSubjectRepository)
    {
        _subjectRepository = subjectRepository;
        _productSubjectRepository = productSubjectRepository;
        _productRepository = productRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Subject kaydını güncelleyen metot.
    /// </summary>
    /// <param name="entity">Güncellenecek subject bilgilerini içeren DTO</param>
    /// <returns>
    /// Başarılı durumda güncellenen subject'in DTO'sunu Success result ile,
    /// Hata durumunda Error result ile döner
    /// </returns>
    public async Task<IDataResult<SubjectApiDto>> UpdateAsync(SubjectUpdateApiDto entity)
    {
        try
        {
            // Name check
            if (await _subjectRepository.AnyAsync(x =>
                x.Name.ToLower().Equals(entity.Name.Trim().ToLower()) &&
                x.Id != entity.Id))
            {
                return new ErrorDataResult<SubjectApiDto>(Messages.SubjectAlreadyExist);
            }

            // Get existing subject
            var subject = await _subjectRepository.GetByIdAsync(entity.Id);
            if (subject is null)
            {
                return new ErrorDataResult<SubjectApiDto>(Messages.SubjectNotFound);
            }

            // Validate products exist
            if (entity.ProductSubjects?.Any() == true)
            {
                foreach (var productSubject in entity.ProductSubjects)
                {
                    if (!await _productRepository.AnyAsync(p => p.Id == productSubject.ProductId))
                    {
                        return new ErrorDataResult<SubjectApiDto>($"Product with id {productSubject.ProductId} not found.");
                    }
                }
            }

            // Update the entity
            subject.Name = entity.Name.Trim();
            subject.ProductSubjects = _mapper.Map<ICollection<ProductSubject>>(entity.ProductSubjects);

            await _subjectRepository.UpdateAsync(subject);
            await _subjectRepository.SaveChangesAsync();

            // Get updated entity and return
            var updatedSubject = await _subjectRepository.GetByIdAsync(entity.Id);
            return new SuccessDataResult<SubjectApiDto>(_mapper.Map<SubjectApiDto>(updatedSubject), Messages.UpdateSuccess);
        }
        catch (Exception ex)
        {
            return new ErrorDataResult<SubjectApiDto>(Messages.InvalidParameter);
        }

    }


    /// <summary>
    /// Belirtilen ID'ye sahip subject kaydını getirir.
    /// </summary>
    /// <param name="id">Getirilecek subject'in ID'si</param>
    /// <returns>
    /// Kayıt bulunursa subject DTO'sunu Success result ile,
    /// Bulunamazsa Error result ile döner
    /// </returns>
    public async Task<IDataResult<SubjectApiDto>> GetByIdAsync(Guid id)
    {
        var subject = await _subjectRepository.GetByIdAsync(id);
        if (subject is null)
        {
            return new ErrorDataResult<SubjectApiDto>(Messages.SubjectNotFound);
        }
        return new SuccessDataResult<SubjectApiDto>(
            _mapper.Map<SubjectApiDto>(subject),
            Messages.FoundSuccess);
    }

    /// <summary>
    /// Yeni bir konu (subject) ekleme işlemini gerçekleştirir.
    /// Gelen verilerin doğruluğunu kontrol eder, eğer tüm ürün ID'leri geçerliyse
    /// ve konu adı daha önce eklenmemişse yeni bir konu oluşturur.
    /// Eğer konu adı daha önce eklenmişse sadece eksik ürün ilişkilerini tamamlar.
    /// </summary>
    /// <param name="entity">Eklenecek konu verilerini içeren DTO (SubjectCreateApiDto).</param>
    /// <returns>
    /// Başarılı olursa eklenen veya güncellenen konu verilerini içeren bir IDataResult<SubjectGetApiDto>.
    /// Hatalı giriş durumunda hata mesajı döner.
    /// </returns>
    public async Task<IDataResult<SubjectApiDto>> AddAsync(SubjectCreateApiDto entity)
    {
        try
        {
            entity.ProductIdList = entity.ProductIdList.Distinct().ToList();
            var subjectExists = await _subjectRepository.AnyAsync(x => x.Name.Trim().ToLower().Equals(entity.Name.Trim().ToLower()));
            bool areAllProductsPresent = false;
            var existingProductIdList = (await _productRepository.GetAllAsync()).Where(x => x.Status == Status.Active || x.Status == Status.Modified || x.Status == Status.Added).Select(x => x.Id).ToList();
            areAllProductsPresent = entity.ProductIdList.All(item => existingProductIdList.Contains(item));

            if (areAllProductsPresent)//eğer gelen product idlerin her biri dbde bulunuyorsa
            {
                List<ProductSubject> newProductSubjects = new List<ProductSubject>();
                Subject addNewSubject = new Subject();
                if (subjectExists)
                {
                    var existingSubject = await _subjectRepository.GetAsync(x => x.Name.Trim().ToLower().Equals(entity.Name.Trim().ToLower()));


                    foreach (var productId in entity.ProductIdList)
                    {
                        var exists = await _productSubjectRepository.AnyAsync(x => x.ProductId.Equals(productId) && x.SubjectId.Equals(existingSubject.Id));

                        if (!exists)
                        {
                            existingSubject.ProductSubjects.Add(new ProductSubject
                            {
                                ProductId = productId,
                                SubjectId = existingSubject.Id,
                            });
                        }
                    }

                    await _subjectRepository.UpdateAsync(existingSubject);
                    await _subjectRepository.SaveChangesAsync();
                    return new SuccessDataResult<SubjectApiDto>(_mapper.Map<SubjectApiDto>(existingSubject), Messages.UpdateSuccess);
                }
                else
                {
                    foreach (var productId in entity.ProductIdList)
                    {
                        newProductSubjects.Add(new ProductSubject
                        {
                            ProductId = productId,
                            SubjectId = addNewSubject.Id,
                        });
                    }
                    addNewSubject.ProductSubjects = newProductSubjects;
                    addNewSubject.Name = entity.Name;
                    await _subjectRepository.AddAsync(addNewSubject);
                    await _subjectRepository.SaveChangesAsync();
                    var mappedEntity = _mapper.Map<SubjectApiDto>(addNewSubject);
                    return new SuccessDataResult<SubjectApiDto>(mappedEntity, Messages.AddSuccess);
                }
            }
            return new ErrorDataResult<SubjectApiDto>(Messages.InvalidParameter);
        }
        catch (Exception ex)
        {
            return new ErrorDataResult<SubjectApiDto>($"Adding failed: {ex.Message}");
        }


    }

    /// <summary>
    /// Tüm konuları listeleme işlemi yapar. // SubjectListDto < // geri dönüş tipi.
    /// </summary>
    /// <returns></returns>
    public async Task<IDataResult<List<SubjectListDto>>> GetAllAsync()
    {
        var subjects = await _subjectRepository.GetAllAsync(false);
        return new SuccessDataResult<List<SubjectListDto>>(_mapper.Map<List<SubjectListDto>>(subjects), Messages.ListedSuccess);
    }
    /// <summary>
    /// ID'ye göre bulunan konunun silme işlemini gerçekleştirir.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<IDataResult<SubjectApiDto>> DeleteAsync(Guid id)
    {
        var subject = await _subjectRepository.GetByIdAsync(id);

        if (subject is null)
        {
            return new ErrorDataResult<SubjectApiDto>(Messages.ProductNotFound);
        }

        subject.Questions.ToList().ForEach(q => q.Status = Core.Enums.Status.Deleted);
        subject.Subtopics.ToList().ForEach(st => st.Status = Core.Enums.Status.Deleted);
        subject.ProductSubjects.ToList().ForEach(ps => ps.Status = Core.Enums.Status.Deleted);

        
        subject.Status = Core.Enums.Status.Deleted;

        await _subjectRepository.DeleteAsync(subject);
        await _subjectRepository.SaveChangesAsync();

        return new SuccessDataResult<SubjectApiDto>(Messages.DeleteSuccess);
    }
}

