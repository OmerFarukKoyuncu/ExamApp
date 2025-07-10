using BAExamApp.DataAccess.Interfaces.Repositories;
using BAExamApp.Dtos.ApiDtos.BranchApiDtos;
using BAExamApp.Dtos.Branches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using static System.Web.Razor.Parser.SyntaxConstants;

namespace BAExamApp.Business.ApiServices.Concrete;
public class BranchApiService : IBranchApiService
{
    private readonly IBranchRepository _branchRepository;
    private readonly IMapper _mapper;
    private readonly IClassroomRepository _classroomRepository;

    public BranchApiService(IBranchRepository branchRepository, IMapper mapper, IClassroomRepository _classroomRepository)
    {
        _branchRepository = branchRepository;
        _mapper = mapper;
        this._classroomRepository = _classroomRepository;
    }



    /// <summary>
    /// Yeni bir şube ekler. 
    /// Gelen şube adı daha önce mevcut ise ekleme işlemi başarısız olur ve bir hata sonucu döner.
    /// </summary>
    /// <param name="branchCreateApiDto">Eklenecek şube için gerekli bilgileri içeren DTO.</param>
    /// <returns>
    /// İşlem sonucu:
    /// - Başarılı ise eklenen şubenin bilgilerini içeren bir <see cref="IDataResult{BranchApiDto}"/> döner.
    [HttpPost]
    public async Task<IDataResult<BranchApiDto>> AddAsync(BranchCreateApiDto branchCreateApiDto)
    {
        var hasBranch = await _branchRepository.AnyAsync(branch => branch.Name.ToLower() == branchCreateApiDto.Name.ToLower());

        if (hasBranch)
        {
            return new ErrorDataResult<BranchApiDto>(Messages.AddFailAlreadyExists);
        }

        var branch = _mapper.Map<Branch>(branchCreateApiDto);

        await _branchRepository.AddAsync(branch);
        await _branchRepository.SaveChangesAsync();

        return new SuccessDataResult<BranchApiDto>(_mapper.Map<BranchApiDto>(branch), Messages.AddSuccess);
    }

    /// <summary>
    /// Şube bir sınıfta kullanılıyorsa pasife alan,
    /// kullanılımıyorsa silen metod
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public async Task<IResult> DeleteAsync(Guid id)
    {
        var branch = await _branchRepository.GetByIdAsync(id);

        if (branch is null)
        {
            return new ErrorDataResult<BranchApiDto>(Messages.BranchNotFound);
        }
        var IsBranchUsed = await IsBranchUsedInClass(branch.Id);

        if (IsBranchUsed)
        {
            branch.Status = Core.Enums.Status.Passive;
            await _branchRepository.SaveChangesAsync();
            return new SuccessResult(Messages.DeleteSuccess);
        }

        await _branchRepository.DeleteAsync(branch);
        await _branchRepository.SaveChangesAsync();

        return new SuccessResult(Messages.DeleteSuccess);
    }

    /// <summary>
    /// belirli bir şubenin aktif bir sınıfta kullanıp kullanılmadığını bool türünde döndüren metod
    /// </summary>
    /// <param name="branchId"></param>
    /// <returns></returns>
    public async Task<bool> IsBranchUsedInClass(Guid branchId)
    {
        var classroom = await _classroomRepository.AnyAsync(c => c.BranchId == branchId && (c.Status == Core.Enums.Status.Active || c.ClosedDate > DateTime.Now));
        return classroom;
    }

    /// <summary>
    /// Tüm şubeleri listeleme işlemi yapar.
    /// </summary>
    /// <returns></returns>
    public async Task<IDataResult<List<BranchListDto>>> GetAllAsync()
    {
        var branches = await _branchRepository.GetAllAsync();
        return new SuccessDataResult<List<BranchListDto>>(_mapper.Map<List<BranchListDto>>(branches), Messages.ListedSuccess);
    }

    /// <summary>
    /// Güncellenecek verinin daha önce sistemde var olup olmadığını kontrol eder eğer yoksa update işlemi yapar.
    /// </summary>
    /// <param name="branchApiUpdateDTO"></param>
    /// <returns></returns>
    public async Task<IDataResult<BranchApiDto>> UpdateAsync(BranchApiUpdateDto branchApiUpdateDTO)
    {
        var hasBranch = await _branchRepository.AnyAsync(branch => branch.Id != branchApiUpdateDTO.Id && branch.Name.ToLower() == branchApiUpdateDTO.Name.ToLower());

        if (hasBranch)
        {
            return new ErrorDataResult<BranchApiDto>(Messages.AddFailAlreadyExists);
        }

        var branch = await _branchRepository.GetByIdAsync(branchApiUpdateDTO.Id);

        if (branch is null)
        {
            return new ErrorDataResult<BranchApiDto>(Messages.BranchNotFound);
        }

        //var updatedBranch = _mapper.Map(branchApiUpdateDTO, branch);
        branch.Name = branchApiUpdateDTO.Name;

        await _branchRepository.UpdateAsync(branch);
        await _branchRepository.SaveChangesAsync();


        return new SuccessDataResult<BranchApiDto>(Messages.UpdateSuccess);
    }

}
