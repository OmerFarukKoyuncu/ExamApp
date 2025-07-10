using BAExamApp.Dtos.Admins;
using BAExamApp.Dtos.Trainers;

namespace BAExamApp.Business.Interfaces.Services;

public interface IAdminService
{
    Task<IDataResult<AdminDto>> GetByIdAsync(Guid id);
    Task<IDataResult<AdminDto>> GetByIdentityIdAsync(string identityId);
    Task<IDataResult<List<AdminListDto>>> GetAllAsync();
    Task<IDataResult<AdminDto>> AddAsync(AdminCreateDto adminCreateDto);
    Task<IResult> AddClassRoomsToTrainers(TrainerAddedToClassroomByAdminDto classroomAddTrainerDto);
    Task<IDataResult<AdminDto>> RoleAddAsync(AdminCreateDto adminCreateDto);
    Task<IDataResult<AdminDto>> UpdateAsync(AdminUpdateDto adminUpdateDto);
    Task<IResult> DeleteAsync(Guid id);
    Task<IDataResult<AdminDetailsDto>> GetDetailsByIdAsync(Guid id);

    /// <summary>
    /// ROLE DEĞİŞTİRME İŞLEMİ İÇİN KULLANILIR STATUS DEĞİŞTİRME İŞLEMİ
    /// </summary>
    /// <param name="identityId"></param>
    /// <param name="make"></param>
    /// <returns></returns>
    Task<IDataResult<AdminDto>> UpdateRoleAsync(string identityId, bool make);
}
