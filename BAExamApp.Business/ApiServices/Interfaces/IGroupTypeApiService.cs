using BAExamApp.Dtos.ApiDtos.GroupTypeApiDtos;
using BAExamApp.Dtos.ApiDtos.SubtopicApiDtos;
using BAExamApp.Dtos.GroupTypes;
using BAExamApp.Dtos.Products;

namespace BAExamApp.Business.ApiServices.Interfaces;

/// <summary>
/// Eğitim tipi ile ilgili işlemleri gerçekleştiren servis arayüzü.
/// </summary>
public interface IGroupTypeApiService
{
    /// <summary>
    /// Eğitim tipi ekleme işlemini yapar.
    /// </summary>
    /// <param name="registerCode"></param>
    /// <param name="groupTypeCreateDto"></param>
    /// <returns></returns>
    Task<IDataResult<GroupTypeApiDto>> AddAsync(GroupTypeCreateApiDto groupTypeCreateApiDto);

    /// <summary>
    /// Belirtilen ID'ye sahip eğitim tipini siler.
    /// </summary>
    /// <param name="id">Silinmek istenen eğitim tipinin ID'si.</param>
    /// <returns>Silme işleminin sonucunu içeren bir IResult nesnesi.</returns>
    Task<IResult> DeleteAsync(Guid id);

    /// <summary>
    /// Tüm eğitim tiplerini listeleme işlemi yapar. // GroupTypeListDto < // geri dönüş tipi.
    /// </summary>
    /// <returns></returns>
    Task<IDataResult<List<GroupTypeListApiDto>>> GetAllAsync();

    Task<IDataResult<GroupTypeApiDto>> UpdateAsync(GroupTypeUpdateApiDto entity);
}
