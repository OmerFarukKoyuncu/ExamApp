using BAExamApp.Dtos.ApiDtos.ExamApiDtos;
using BAExamApp.Dtos.ApiDtos.ExamDtos;
using BAExamApp.Dtos.ApiDtos.StudentExamApiDtos;
using BAExamApp.Dtos.Exams;
using BAExamApp.Dtos.StudentExams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Business.ApiServices.Interfaces;
public interface IExamApiService
{
    /// <summary>
    /// Öğrencinin adını, soyadıni, mailini, sınıfını, girdiği sınavın adını, skorunu ve kuralını döndürür.
    /// </summary>
    /// <returns></returns>
    Task<IDataResult<List<GetAllDataWithRegisterCodeDto>>> GetAllDataWithRegisterCodeAsync();

    /// <summary>
    /// <see cref="ExamType"/> enum'undaki değerleri alır ve her birini 
    /// ID ve isim bilgisi içeren bir listeye dönüştürür.
    /// </summary>
    /// <returns>
    /// Başarı durumunda sınav türleri listesi, aksi halde hata mesajı döner.
    /// </returns>
    Task<IDataResult<List<ExamTypeDto>>> GetExamTypesAsync();


    /// <summary>
    /// `ExamCreationType` enum değerlerini asenkron olarak alır, her bir değeri `ExamCreateTypeDto` nesnesine dönüştürür 
    /// ve bir liste olarak döndürür.
    /// </summary>
    /// <returns>
    /// `IDataResult` ile bir liste döner:
    /// - Başarılı durumda: `SuccessDataResult<List<ExamCreateTypeDto>>` (Liste ve başarı mesajı içerir).
    /// - Hata durumunda: `ErrorDataResult<List<ExamCreateTypeDto>>` (Hata mesajı ile birlikte null değer döner).
    /// </returns>
    Task<IDataResult<List<ExamCreateTypeDto>>> GetExamCreateTypesAsync();


    /// <summary>
    /// Sınav başlatma işlevini yapar.
    /// Sınav başlatmak için parametre olarak ilgili sınavın Guid Id'sini ve öğrenciye gönderilecek sınav link içeriğini alır.
    /// </summary>
    /// <param name="examId">Sınavın GUID ID'si</param>
    /// <param name="link">Öğrenciye gönderilecek sınav link içeriği</param>
    /// <returns>
    /// Başarılı olursa Dönüş olarak SuccessDataResult<ExamListDto>> Tipinde veri dönüşü olur
    /// Başarısız olursa ErrorDataResult<List<ExamListDto> Tipinde veri dönüşü olur
    /// </ExamListDto></returns>
    Task<IDataResult<List<ExamListDto>>> StartExam(Guid examId, string link);

    /// <summary>
    /// Bir sınav oluşturur, `ExamCreateDto` nesnesini `Exam` varlığına dönüştürür 
    /// ve veritabanına ekler. Başarıyla tamamlandığında oluşturulan sınavı döner.
    /// </summary>
    /// <param name="examCreateDto">Oluşturulacak sınavın bilgilerini içeren DTO.</param>
    /// <returns>
    /// `IDataResult` ile oluşturulan sınav döner:  
    /// - Başarılı durumda: `SuccessDataResult<ExamDto>` (Sınav detayları ve başarı mesajı içerir).  
    /// - Hata durumunda: `ErrorDataResult<ExamDto>` (Hata mesajı ile birlikte null değer döner).  
    /// </returns>
    Task<IDataResult<ExamDto>> CreateExamAsync(ExamCreateDto examCreateDto);

    /// <summary>
    /// Parametredeki ExamId'ye ait olan Sınava giren tüm öğrencilerin sonuçlarını döndürür.
    /// <IEnumerable<StudentExamListDto>> Tipinde
    /// </summary>
    /// <param name="examId"></param>
    /// <returns></returns>
    Task<IDataResult<IEnumerable<StudentExamListDto>>> GetExamResultForStudentsByExamId(Guid examId);
}
