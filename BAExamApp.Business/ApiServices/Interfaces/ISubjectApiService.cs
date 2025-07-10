using BAExamApp.Dtos.ApiDtos.SubjectApiDtos;
using BAExamApp.Dtos.Subjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Business.ApiServices.Interfaces;

/// <summary>
/// Subject işlemlerini yöneten servis arayüzü
/// </summary>
public interface ISubjectApiService
{
    /// <summary>
    /// Subject kaydını günceller
    /// </summary>
    /// <param name="entity">Güncellenecek subject bilgileri</param>
    Task<IDataResult<SubjectApiDto>> UpdateAsync(SubjectUpdateApiDto entity);

    /// <summary>
    /// ID'ye göre subject kaydını getirir
    /// Yeni bir konu (subject) ekleme işlemini gerçekleştirir.
    /// Gelen verilerin doğruluğunu kontrol eder, eğer tüm ürün ID'leri geçerliyse
    /// ve konu adı daha önce eklenmemişse yeni bir konu oluşturur.
    /// Eğer konu adı daha önce eklenmişse sadece eksik ürün ilişkilerini tamamlar.
    /// </summary>
    /// <param name="id">Getirilecek subject'in ID'si</param>
    Task<IDataResult<SubjectApiDto>> GetByIdAsync(Guid id);
    /// <param name="entity">Eklenecek konu verilerini içeren DTO (SubjectCreateApiDto).</param>
    /// <returns>
    /// Başarılı olursa eklenen veya güncellenen konu verilerini içeren bir IDataResult<SubjectApiDto>.
    /// Hatalı giriş durumunda hata mesajı döner.
    /// </returns>
    Task<IDataResult<SubjectApiDto>> AddAsync(SubjectCreateApiDto entity);
    /// <summary>
    /// Tüm konuları listeleme işlemi yapar. // SubjectListDto < // geri dönüş tipi.
    /// </summary>
    /// <returns></returns>
    Task<IDataResult<List<SubjectListDto>>> GetAllAsync();
    /// <summary>
    /// ID'ye göre bulunan konunun silme işlemini gerçekleştirir.
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    Task<IDataResult<SubjectApiDto>> DeleteAsync(Guid id);
}
