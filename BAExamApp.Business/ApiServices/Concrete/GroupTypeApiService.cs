using BAExamApp.Core.Entities.Interfaces;
using BAExamApp.DataAccess.Interfaces.Repositories;
using BAExamApp.Dtos.ApiDtos.GroupTypeApiDtos;
using BAExamApp.Dtos.ApiDtos.SubtopicApiDtos;
using BAExamApp.Dtos.GroupTypes;
using BAExamApp.Dtos.Products;
using BAExamApp.Dtos.Subjects;

namespace BAExamApp.Business.ApiServices.Concrete;

/// <summary>
/// Eğitim tipi ile ilgili işlemleri gerçekleştiren servis.
/// </summary>
public class GroupTypeApiService : IGroupTypeApiService
{
    private readonly IMapper _mapper;
    private readonly IGroupTypeRepository _groupTypeRepository;

    public GroupTypeApiService(IGroupTypeRepository groupTypeRepository, IMapper mapper)
    {
        _groupTypeRepository = groupTypeRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Eğitim tipi ekleme işlemini yapar.
    /// </summary>
    /// <param name="groupTypeCreateDto"></param>
    /// <returns></returns>
    public async Task<IDataResult<GroupTypeApiDto>> AddAsync(GroupTypeCreateApiDto groupTypeCreateApiDto)
    {
        try
        {
            var hasGroupType = await _groupTypeRepository.AnyAsync(x => x.Name.ToLower().Equals(groupTypeCreateApiDto.Name.ToLower()));

            if (hasGroupType)
            {
                return new ErrorDataResult<GroupTypeApiDto>(Messages.AddFailAlreadyExists);
            }

            var groupType = _mapper.Map<GroupType>(groupTypeCreateApiDto);

            await _groupTypeRepository.AddAsync(groupType);
            await _groupTypeRepository.SaveChangesAsync();

            return new SuccessDataResult<GroupTypeApiDto>(_mapper.Map<GroupTypeApiDto>(groupType), Messages.AddSuccess);
        }
        catch (Exception)
        {
            return new ErrorDataResult<GroupTypeApiDto>(Messages.AddFail);
        }
    }

    /// <summary>
    /// Belirtilen ID'ye sahip eğitim tipini siler.
    /// </summary>
    /// <param name="id">Silinmek istenen eğitim tipinin ID'si.</param>
    /// <returns>Silme işleminin sonucunu içeren bir IResult nesnesi.</returns>
    public async Task<IResult> DeleteAsync(Guid id)
    {
        var groupType = await _groupTypeRepository.GetByIdAsync(id);

        if (groupType is null)
        {
            return new ErrorResult(Messages.GroupTypeNotFound);
        }

        await _groupTypeRepository.DeleteAsync(groupType);
        await _groupTypeRepository.SaveChangesAsync();

        return new SuccessResult(Messages.DeleteSuccess);
    }

    /// <summary>
    /// Tüm eğitim tiplerini listeleme işlemi yapar. // GroupTypeListDto < // geri dönüş tipi.
    /// </summary>
    /// <returns></returns>
    public async Task<IDataResult<List<GroupTypeListApiDto>>> GetAllAsync()
    {
        var grouptypes = await _groupTypeRepository.GetAllAsync(false);
        return new SuccessDataResult<List<GroupTypeListApiDto>>(_mapper.Map<List<GroupTypeListApiDto>>(grouptypes), Messages.ListedSuccess);
    }

    /// <summary>
    /// Verilen GroupTypeUpdateApiDto nesnesine göre bir grup türünü günceller.
    /// Eğer grup türü bulunamazsa hata döner.
    /// Boş isim veya bilgi alanları varsa, mevcut değerler korunur.
    /// Güncelleme başarılı olursa, güncellenmiş grup türü döner.
    /// </summary>

    public async Task<IDataResult<GroupTypeApiDto>> UpdateAsync(GroupTypeUpdateApiDto entity)
    {
        try
        {
            var existingGroupType = await _groupTypeRepository.GetByIdAsync(entity.Id);
            if (existingGroupType is null)
            {
                return new ErrorDataResult<GroupTypeApiDto>(Messages.GroupTypeNotFound);
            }

            if (string.IsNullOrWhiteSpace(entity.Name))
            {
                entity.Name = existingGroupType.Name;
            }
            if (string.IsNullOrWhiteSpace(entity.Information))
            {
                entity.Information = existingGroupType.Information;
            }


            var updatedGroupType = _mapper.Map(entity, existingGroupType);
            await _groupTypeRepository.UpdateAsync(updatedGroupType);
            await _groupTypeRepository.SaveChangesAsync();
            var mappedResult = _mapper.Map<GroupTypeApiDto>(updatedGroupType);
            return new SuccessDataResult<GroupTypeApiDto>(mappedResult, Messages.UpdateSuccess);
        }
        catch (Exception)
        {
            return new ErrorDataResult<GroupTypeApiDto>(Messages.UpdateFail);
        }
    }
}
