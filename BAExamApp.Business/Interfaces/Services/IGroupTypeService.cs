using BAExamApp.Dtos.GroupTypes;

namespace BAExamApp.Business.Interfaces.Services;

public interface IGroupTypeService
{
    Task<IDataResult<GroupTypeDto>> GetByIdAsync(Guid id);
    Task<IDataResult<List<GroupTypeListDto>>> GetAllAsync();
    Task<IDataResult<GroupTypeDto>> AddAsync(GroupTypeCreateDto groupTypeCreateDto);

    /// <summary>
    /// Eğitim tipini mevcutsa ona ait olan sınıflar ile beraber döndürür.
    /// </summary>
    /// <param name="id">İlgili eğitim tipini bulmak için GroupTypeId gereklidir.</param>
    /// <returns>GroupTypeDto</returns>
    Task<IDataResult<GroupTypeDto>> GetByIdWithClassroomsAsync(Guid id);

    /// <summary>
    /// Verilen GroupTypeUpdateDto nesnesine göre bir grup türünü günceller.
    /// Güncelleme işlemi sonucunda IDataResult<GroupTypeDto> döner.
    /// </summary>
    Task<IDataResult<GroupTypeDto>> UpdateAsync(GroupTypeUpdateDto groupTypeUpdateDto);
    Task<IResult> DeleteAsync(Guid id);
}
