using BAExamApp.Business.Services;
using BAExamApp.DataAccess.Interfaces.Repositories;
using BAExamApp.Dtos.ApiDtos.ExamApiDtos;
using BAExamApp.Dtos.ApiDtos.ExamDtos;
using BAExamApp.Dtos.ApiDtos.StudentExamApiDtos;
using BAExamApp.Dtos.Exams;
using BAExamApp.Dtos.SendMails;
using BAExamApp.Dtos.StudentClassrooms;
using BAExamApp.Dtos.Exams;
using BAExamApp.Entities.Enums;
using Hangfire;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BAExamApp.Dtos.StudentExams;

namespace BAExamApp.Business.ApiServices.Concrete;
public class ExamApiService : IExamApiService
{
    private readonly IExamRepository _examRepository;
    private readonly IExamService _examService;
    private readonly IMapper _mapper;
    private readonly IStudentExamService _studentExamService;

    public ExamApiService(IExamRepository examRepository, IMapper mapper, IExamService examService, IStudentExamService studentExamService)
    {
        _examRepository = examRepository;
        _examService = examService;
        _mapper = mapper;
        _studentExamService = studentExamService;
    }

    /// <summary>
    /// Öğrencinin adını, soyadıni, mailini, sınıfını, girdiği sınavın adını, skorunu ve kuralını döndürür.
    /// </summary>
    /// <returns></returns>
    public async Task<IDataResult<List<GetAllDataWithRegisterCodeDto>>> GetAllDataWithRegisterCodeAsync()
    {
        var values = (await _examRepository.GetAllAsync()).Select(x => new GetAllDataWithRegisterCodeDto
        {
            ExamName = x.Name,
            ExamRule = x.ExamRule?.Name,
            ExamClassroom = x.ExamClassrooms?.FirstOrDefault(y => y.ExamId == x.Id)?.Classroom?.Name,
            StudentInfo = x.StudentExams.Select(se => new StudentInfoAndScoreDto
            {
                StudentFirstName = se.Student?.FirstName,
                StudentLastName = se.Student?.LastName,
                StudentEmail = se.Student?.Email,
                ExamScore = se.Score
            }).ToList()
        }).ToList();

        if (values is not null) return new SuccessDataResult<List<GetAllDataWithRegisterCodeDto>>(values, Messages.FoundSuccess);
        return new ErrorDataResult<List<GetAllDataWithRegisterCodeDto>>(values, Messages.ListNotFound);
    }


    /// <summary>
    /// <see cref="ExamType"/> enum'undaki mevcut sınav türlerini döndürür.
    /// </summary>
    /// <returns>
    /// Sınav türlerinin listesini içeren bir veri sonucu döndürür.
    /// - Eğer sınav türleri başarıyla alınırsa, sınav türlerinin listesini ve bir başarı mesajını içeren <see cref="SuccessDataResult{T}"/> döner.
    /// - Eğer sınav türleri bulunamazsa, null değer ve bir hata mesajı içeren <see cref="ErrorDataResult{T}"/> döner.
    /// </returns>
    /// <remarks>
    /// Bu metot, <see cref="ExamType"/> enum'undaki değerleri alır ve her birini <see cref="ExamTypeDto"/> nesnesine dönüştürür.
    /// ID ve varsa Display adı gibi bilgileri içerir.
    /// </remarks>

    public async Task<IDataResult<List<ExamTypeDto>>> GetExamTypesAsync()
    {
        var examTypes = Enum.GetValues(typeof(ExamType))
            .Cast<ExamType>()
            .Select(e => new ExamTypeDto
            {
                Id = (int)e,
                Name = e.GetType()
                        .GetMember(e.ToString())
                        .First()
                        .GetCustomAttributes(typeof(DisplayAttribute), false)
                        .Cast<DisplayAttribute>()
                        .FirstOrDefault()?.Name ?? e.ToString()
            }).ToList();

        if (examTypes.Any())
        {
            return new SuccessDataResult<List<ExamTypeDto>>(examTypes, Messages.FoundSuccess);
        }
        return new ErrorDataResult<List<ExamTypeDto>>(null, Messages.ListNotFound);
    }

    /// <summary>
    /// `ExamCreationType` enum değerlerini asenkron olarak alır, her bir değeri `ExamCreateTypeDto` nesnesine dönüştürür 
    /// ve bir liste olarak döndürür.
    /// </summary>
    /// <returns>
    /// `IDataResult` ile bir liste döner:
    /// - Başarılı durumda: `SuccessDataResult<List<ExamCreateTypeDto>>` (Liste ve başarı mesajı içerir).
    /// - Hata durumunda: `ErrorDataResult<List<ExamCreateTypeDto>>` (Hata mesajı ile birlikte null değer döner).
    /// </returns>
    public async Task<IDataResult<List<ExamCreateTypeDto>>> GetExamCreateTypesAsync()
    {
        try
        {
            // Asenkron işlemleri simüle etmek için Task.Run kullanabilirsiniz
            var examTypes = await Task.Run(() =>
            {
                return Enum.GetValues(typeof(ExamCreationType))
                           .Cast<ExamCreationType>()
                           .Select(e => new ExamCreateTypeDto
                           {
                               Id = (int)e,
                               Name = e.ToString()
                           })
                           .ToList();
            });

            return new SuccessDataResult<List<ExamCreateTypeDto>>(examTypes, Messages.FoundSuccess);
        }
        catch (Exception ex)
        {
            return new ErrorDataResult<List<ExamCreateTypeDto>>(null, Messages.ListNotFound + $"Hata: {ex.Message}");
        }

    }

    /// <summary>
    /// Yeni bir sınav oluşturur. `ExamCreateDto` verilerini `Exam` varlığına dönüştürerek veritabanına ekler.  
    /// Sınav başarıyla eklendiğinde oluşturulan sınav bilgilerini döner.
    /// </summary>
    /// <param name="examCreateDto">Oluşturulacak sınavın bilgilerini içeren DTO.</param>
    /// <returns>
    /// `IDataResult` ile sınav bilgilerini döner:  
    /// - Başarılı durumda: `SuccessDataResult<ExamDto>` (Sınav detayları ve başarı mesajı içerir).  
    /// - Hata durumunda: `ErrorDataResult<ExamDto>` (Hata mesajı ile birlikte null değer döner).  
    /// </returns>
    public async Task<IDataResult<ExamDto>> CreateExamAsync(ExamCreateDto examCreateDto)
    {
        if (string.IsNullOrWhiteSpace(examCreateDto.Name))
        {
            return new ErrorDataResult<ExamDto>(null, Messages.InvalidParameter);
        }

        if (examCreateDto.ExamDateTime == default)
        {
            return new ErrorDataResult<ExamDto>(null, Messages.InvalidParameter);
        }

        var examEntity = _mapper.Map<Exam>(examCreateDto);

       
        examEntity.EndExamTime = examCreateDto.ExamDateTime.Add(examCreateDto.ExamDuration);
        examEntity.IsStarted = false;
        examEntity.IsCanceled = false;

       
        var result = await _examRepository.AddAsync(examEntity);
        if (result == null)
        {
            return new ErrorDataResult<ExamDto>(null, Messages.AddFail);
        }

        await _examRepository.SaveChangesAsync();

        var examDto = _mapper.Map<ExamDto>(examEntity);

        return new SuccessDataResult<ExamDto>(examDto, Messages.ExamCreatedSuccessfully);
    }

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
    public async Task<IDataResult<List<ExamListDto>>> StartExam(Guid examId, string link)
    {
        var examStartResult = await _examService.GetStudentsInfosByExamIdAsync(examId, link);
        var examStarted = await _examService.GetByIdAsync(examId);
        if (!examStarted.IsSuccess)
            return new ErrorDataResult<List<ExamListDto>>(examStartResult.Data, Messages.ExamStartFail);
        return new SuccessDataResult<List<ExamListDto>>(examStartResult.Data, examStartResult.Message);

    }

    /// <summary>
    /// Parametredeki ExamId'ye ait olan Sınava giren tüm öğrencilerin sonuçlarını döndürür.
    /// Başarısız olma durumunda ErrorDataResult<IEnumerable<StudentExamListDto>>(examResultStudents.Data, Messages.StudentExamsNotFound) döndürür.
    /// Başarılı olma durumunda SuccessDataResult<IEnumerable<StudentExamListDto>>(examResultStudents.Data, Messages.StudentExamFoundSuccess) döndürür.
    /// </summary>
    /// <param name="examId"></param>
    /// <returns></returns>
    public async Task<IDataResult<IEnumerable<StudentExamListDto>>> GetExamResultForStudentsByExamId(Guid examId)
    {
        var examResultStudents = await _studentExamService.GetAllByExamIdAsync(examId);
        if (!examResultStudents.IsSuccess) return new ErrorDataResult<IEnumerable<StudentExamListDto>>(examResultStudents.Data, Messages.StudentExamsNotFound);
        return new SuccessDataResult<IEnumerable<StudentExamListDto>>(examResultStudents.Data, Messages.StudentExamFoundSuccess);
    }
}
