using BAExamApp.Dtos.ApiDtos.BranchApiDtos;
using BAExamApp.Dtos.Branches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Business.ApiServices.Interfaces;
public interface IBranchApiService
{
    /// <summary>
    /// Şube bir sınıfta kullanılıyorsa pasife alan,
    /// kullanılımıyorsa silen metod
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<IResult> DeleteAsync(Guid id);

    /// <summary>
    /// belirli bir şubenin aktif bir sınıfta kullanıp kullanılmadığını bool türünde döndüren metod
    /// </summary>
    /// <param name="branchId"></param>
    /// <returns></returns>
    Task<bool> IsBranchUsedInClass(Guid branchId);
    /// <summary>
    /// Yeni bir şube ekler. 
    /// Gelen şube adı daha önce mevcut ise ekleme işlemi başarısız olur ve bir hata sonucu döner.
    /// </summary>
    /// <param name="branchCreateApiDto">Eklenecek şube için gerekli bilgileri içeren DTO.</param>
    /// <returns>
    /// İşlem sonucu:
    /// - Başarılı ise eklenen şubenin bilgilerini içeren bir <see cref="IDataResult{BranchApiDto}"/> döner.
    Task<IDataResult<BranchApiDto>> AddAsync(BranchCreateApiDto branchCreateApiDto);

    /// <summary>
    /// Bütün şubeleri Dto tipinde asenkron olarak getirir.
    /// </summary>
    /// <returns></returns>
    Task<IDataResult<List<BranchListDto>>> GetAllAsync();

    /// <summary>
    /// Güncellenecek verinin daha önce sistemde var olup olmadığını kontrol eder eğer yoksa update işlemi yapar.
    /// </summary>
    /// <param name="branchApiUpdateDTO"></param>
    /// <returns></returns>
    Task<IDataResult<BranchApiDto>> UpdateAsync(BranchApiUpdateDto branchApiUpdateDTO);

}
