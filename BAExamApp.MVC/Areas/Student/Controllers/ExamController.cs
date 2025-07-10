using AutoMapper;
using BAExamApp.Business.Constants;
using BAExamApp.Business.Services;
using BAExamApp.Dtos.Emails;
using BAExamApp.Dtos.SendMails;
using BAExamApp.Dtos.StudentAnswers;
using BAExamApp.Dtos.StudentExams;
using BAExamApp.Dtos.StudentQuestions;
using BAExamApp.Entities.DbSets;
using BAExamApp.Entities.Enums;
using BAExamApp.MVC.Areas.Student.Models.ExamVMs;
using BAExamApp.MVC.Areas.Student.Models.StudentExamVMs;
using BAExamApp.MVC.Areas.Student.Models.StudentQuestionVMs;
using BAExamApp.MVC.Areas.Trainer.Models.QuestionAnswerVMs;
using Microsoft.AspNetCore.Authorization;
using System.Drawing.Printing;
using System.Text.Json;
using X.PagedList;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace BAExamApp.MVC.Areas.Student.Controllers;
public class ExamController : StudentBaseController
{
    private readonly IMapper _mapper;
    private readonly IStudentService _studentService;
    private readonly IStudentExamService _studentExamService;
    private readonly IStudentQuestionService _studentQuestionService;
    private readonly IQuestionAnswerService _questionAnswerService;
    private readonly IExamService _examService;
    private readonly IExamAnalysisService _examAnalysisService;
    private readonly ISendMailService _sendMailService;

    public ExamController(IMapper mapper, IStudentService studentService, IStudentExamService studentExamService, IStudentQuestionService studentQuestionService, IExamService examService, IQuestionAnswerService questionAnswerService, IExamAnalysisService examAnalysisService, ISendMailService sendMailService)
    {
        _mapper = mapper;
        _studentService = studentService;
        _studentExamService = studentExamService;
        _studentQuestionService = studentQuestionService;
        _examService = examService;
        _questionAnswerService = questionAnswerService;
        _examAnalysisService = examAnalysisService;
        _sendMailService = sendMailService;
    }


    /// <summary>
    /// Giriş yapmış kullanıcının öğrenci kimliğini alarak, bu kullanıcıya ait sınav listesini çeker.
    /// Liste, sınav tarihine göre azalan şekilde sıralanır ve belirtilen sayfa numarasına göre sayfalanır.
    /// Veriler ViewModel'e dönüştürülerek View'a gönderilir.
    /// Eğer kullanıcı kimliği alınamazsa Unauthorized sonucu döner.
    /// </summary>
    public async Task<IActionResult> Index(int? page, int pageSize = 10)
    {
        if (string.IsNullOrEmpty(UserId))
        {
            return Unauthorized();
        }

        var studentId = Guid.Parse(UserId);
        var result = await _studentExamService.GetExamListByStudentIdAsync(studentId);

        int pageNumber = page ?? 1;

        var examList = _mapper.Map<List<StudentExamListVM>>(result?.Data ?? new List<StudentsExamsDto>());

        var paginatedList = examList
          .OrderByDescending(e => e.ExamDateTime)
          .ToPagedList(pageNumber, pageSize);

        ViewBag.PageSize = pageSize;
        ViewBag.CurrentPage = pageNumber;

        return View(paginatedList);
    }

    /// <summary>
    /// Öğrencinin link yardımı ile sınav başlangıç saati gelmiş olan sınava girmesini sağlar.
    /// </summary>
    /// <param name="id">Öğrencinin sınav başlangıç saati gelmiş olan sınavına ait StudentExam id'si</param>
    /// <returns>StartExam View'i döner</returns>
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> StartExam(Guid id)
    {
        var studentExamResult = await _studentExamService.GetByIdAsync(id);

        if (studentExamResult.IsSuccess)
        {
            var studentExam = studentExamResult.Data;

            var examResult = await _examService.GetByIdAsync(studentExam.ExamId);

            if (!studentExam.IsReadRules && !(DateTime.Now > examResult.Data.ExamDateTime + examResult.Data.ExamDuration))
            {
                studentExam.IsReadRules = true;
                var mappedStudentExam = _mapper.Map<StudentExamUpdateDto>(studentExam);
                await _studentExamService.UpdateAsync(mappedStudentExam);
                return View("ExamRules");
            }
            else if (!studentExam.IsReadRules && studentExam.RetakeExam) //Mazeretli Öğrencilerin kontrolünü yapıyoruz.
            {
                studentExam.IsReadRules = true;
                studentExam.RetakeExamDate = studentExamResult.Data.RetakeExamDate;
                studentExam.IsFinished = false;
                var mappedStudentExam = studentExam.Adapt<StudentExamUpdateDto>();
                await _studentExamService.UpdateAsync(mappedStudentExam);
                return View("ExamRules");

            }

            if (examResult.IsSuccess)
            {
                if (!studentExam.RetakeExam) //Mazereti !OLMAYAN! öğrencilerin sınavlarını baz alıyoruz.
                {
                    var exam = examResult.Data;
                    ViewBag.ExamDateTime = exam.ExamDateTime;

                    //Öğrencinin sınavı tamamlayıp tamamlamadığını, sınavın başlayıp başlamadığını ve sınavın bitip bitmediğini denetler.                
                    if (!studentExam.IsFinished && DateTime.Now >= exam.ExamDateTime 
                        && DateTime.Now < exam.EndExamTime.AddMinutes(1))
                    {
                        var model = _mapper.Map<StudentStudentExamStartVM>(studentExam);
                        model = _mapper.Map(exam, model);
                        model.EndExamTime = DateTime.Now;
                        return View(model);
                    }
                    else if (studentExam.IsFinished)
                    {
                        return RedirectToAction("GetExamResult", new { studentExamId = id });
                    }
                    else if (!studentExam.IsFinished && (DateTime.Now > exam.ExamDateTime + exam.ExamDuration || DateTime.Now > exam.EndExamTime))
                    {
                        return RedirectToAction("MissExam", new { id = id });
                    }
                    else
                    {
                        ViewBag.ErrorMessage = "Exam_Not_Started";
                    }

                }
                else
                {
                    //Mazeret sınavı için gerekli işlemler!
                    var retakeExam = examResult.Data;
                    ViewBag.ExamDateTime = studentExam.RetakeExamDate;
                    if (!studentExam.IsFinished && DateTime.Now <= studentExam.RetakeExamDate + retakeExam.ExamDuration)
                    {
                        var model = _mapper.Map<StudentStudentExamStartVM>(studentExam);
                        model = _mapper.Map(retakeExam, model);
                        model.ExamDateTime = studentExam.RetakeExamDate;
                        return View(model);
                    }
                    else if (studentExam.IsFinished)
                    {
                        return RedirectToAction("GetExamResult", new { studentExamId = id });
                    }
                    else if (!studentExam.IsFinished && DateTime.Now > studentExam.RetakeExamDate + retakeExam.ExamDuration)
                    {
                        return RedirectToAction("MissExam", new { id = id });
                    }
                    ViewBag.ErrorMessage = "Exam_Not_Started";
                }

            }
            else
                ViewBag.ErrorMessage = "Exam_Not_Found";
        }
        else
            ViewBag.ErrorMessage = "Exam_Not_Found";
        return View();
    }

    /// <summary>
    /// Öğrencinin bir buton yardımı ile sınavı başlatmasını, sınav başlamışsa kaldığı sorudan devam etmesini sağlar.
    /// </summary>
    /// <param name="model">StudentExamId'yi çektiğimiz "StudentStudentExamStartVM" modeli</param>
    /// <returns>GetNextQuestion action'ını döner</returns>
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> StartExam(StudentStudentExamStartVM model)
    {
        return RedirectToAction("GetNextQuestion", new { studentExamId = model.StudentExamId });
    }

    /// <summary>
    /// Öğrencinin sınavındaki sıradaki soruya ait bilgileri çekip öğrenciye gösterir.
    /// </summary>
    /// <param name="studentExamId">Öğrencinin sınav başlangıç saati gelmiş olan sınavına ait StudentExam id'si</param>
    /// <returns>GetNextQuestion view'ını veya StartExam action'ını döner</returns>
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetNextQuestion(Guid studentExamId)
    {
        var studentExamResult = await _studentExamService.GetByIdAsync(studentExamId);
        var answerList = new List<bool>();
        if (studentExamResult.IsSuccess)
        {
            var studentExam = studentExamResult.Data;
            var examResult = await _examService.GetByIdAsync(studentExam.ExamId);

            // Soru cevaplama durumlarını kontrol et
            for (int i = 0; i < studentExam.StudentQuestions.Count; i++)
            {
                var selected = false;
                if (studentExam.StudentQuestions[i].StudentAnswers.Count > 0)
                {
                    for (int j = 0; j < studentExam.StudentQuestions[i].StudentAnswers.Count; j++)
                    {
                        if (studentExam.StudentQuestions[i].StudentAnswers[j].IsSelected == true)
                        {
                            selected = true;
                            break;
                        }
                    }
                }
                answerList.Add(selected);
            }
            ViewBag.answerList = answerList;

            if (examResult.IsSuccess)
            {
                var exam = examResult.Data;
                ViewBag.ExamTypeJSON = JsonSerializer.Serialize(exam.ExamType);

                if (studentExam.StartTime == null)
                {
                    var updateDto = _mapper.Map<StudentExamUpdateDto>(studentExam);
                    updateDto.StartTime = DateTime.Now;
                    await _studentExamService.UpdateAsync(updateDto);
                    studentExam.StartTime = DateTime.Now; // Memory'deki objeyi de güncelle
                }

                if (!studentExam.RetakeExam) //Normal sınav durumu
                {
                    ViewBag.ExamDate = exam.ExamDateTime;
                    ViewBag.ExamName = exam.Name;

                    var examEndTime = exam.ExamDateTime + exam.ExamDuration;
                    if (!studentExam.IsFinished && DateTime.Now >= exam.ExamDateTime && DateTime.Now <= examEndTime)
                    {
                        if (studentExam.AnsweredQuestionCount < studentExam.StudentQuestions.Count)
                        {
                            var studentQuestionResult = await _studentQuestionService.GetByStudentExamIdAndQuestionOrderAsync(studentExamId, studentExam.AnsweredQuestionCount + 1);

                            if (studentQuestionResult.IsSuccess)
                            {
                                var studentQuestion = studentQuestionResult.Data;
                                if (studentQuestion.Image != null)
                                    studentQuestion.TimeGiven = studentQuestion.TimeGiven.Add(TimeSpan.FromSeconds(5));

                                if (studentQuestion.TimeStarted != null && studentQuestion.TimeStarted + studentQuestion.TimeGiven < DateTime.Now)
                                {
                                    var mappedExam = _mapper.Map<StudentExamUpdateDto>(studentExam);
                                    mappedExam.AnsweredQuestionCount++;

                                    if (mappedExam.AnsweredQuestionCount > studentExam.StudentQuestions.Count || DateTime.Now >= examEndTime)
                                    {
                                        mappedExam.IsFinished = true;
                                        var studentExamScore = await CalculateStudentQuestionScoresAndExamScore(studentExam);
                                        mappedExam.Score = studentExamScore > exam.MaxScore ? exam.MaxScore : studentExamScore;
                                    }
                                    await _studentExamService.UpdateAsync(mappedExam);

                                    studentQuestion.TimeFinished = DateTime.Now;
                                    var mappedQuestion = _mapper.Map<StudentQuestionUpdateDto>(studentQuestion);
                                    await _studentQuestionService.UpdateAsync(mappedQuestion);

                                    return RedirectToAction("GetNextQuestion", new { studentExamId = studentExamId });
                                }
                                else if (studentQuestion.TimeStarted == null)
                                {
                                    studentQuestion.TimeStarted = DateTime.Now;
                                    var mappedQuestion = _mapper.Map<StudentQuestionUpdateDto>(studentQuestion);
                                    await _studentQuestionService.UpdateAsync(mappedQuestion);
                                }

                                var model = _mapper.Map<StudentStudentQuestionDetailsVM>(studentQuestion);
                                model.QuestionCount = studentExam.StudentQuestions.Count;
                                model.ExamDateTime = studentExam.StartTime ?? DateTime.Now;
                                return View(model);
                            }
                            else
                                ViewBag.ErrorMessage = studentQuestionResult.Message;
                        }
                    }
                }
                else //Mazeret sınavı durumu
                {
                    ViewBag.ExamDate = studentExam.RetakeExamDate;
                    ViewBag.ExamName = exam.Name;

                    // Mazeret sınavı süre kontrolü
                    var retakeExamEndTime = studentExam.RetakeExamDate + exam.ExamDuration;
                    if (!studentExam.IsFinished && DateTime.Now >= studentExam.RetakeExamDate && DateTime.Now <= retakeExamEndTime)
                    {
                        if (studentExam.AnsweredQuestionCount < studentExam.StudentQuestions.Count)
                        {
                            var studentQuestionResult = await _studentQuestionService.GetByStudentExamIdAndQuestionOrderAsync(studentExamId, studentExam.AnsweredQuestionCount + 1);

                            if (studentQuestionResult.IsSuccess)
                            {
                                var studentQuestion = studentQuestionResult.Data;
                                if (studentQuestion.Image != null)
                                    studentQuestion.TimeGiven = studentQuestion.TimeGiven.Add(TimeSpan.FromSeconds(5));

                                if (studentQuestion.TimeStarted != null && studentQuestion.TimeStarted + studentQuestion.TimeGiven < DateTime.Now)
                                {
                                    var mappedExam = _mapper.Map<StudentExamUpdateDto>(studentExam);
                                    mappedExam.AnsweredQuestionCount++;

                                    // Bitiş kontrolü - mazeret sınavı için retakeExamEndTime kullanılıyor
                                    if (mappedExam.AnsweredQuestionCount > studentExam.StudentQuestions.Count || DateTime.Now >= retakeExamEndTime)
                                    {
                                        mappedExam.IsFinished = true;
                                        var studentExamScore = await CalculateStudentQuestionScoresAndExamScore(studentExam);
                                        mappedExam.Score = studentExamScore > exam.MaxScore ? exam.MaxScore : studentExamScore;
                                    }
                                    await _studentExamService.UpdateAsync(mappedExam);

                                    studentQuestion.TimeFinished = DateTime.Now;
                                    var mappedQuestion = _mapper.Map<StudentQuestionUpdateDto>(studentQuestion);
                                    await _studentQuestionService.UpdateAsync(mappedQuestion);

                                    return RedirectToAction("GetNextQuestion", new { studentExamId = studentExamId });
                                }
                                else if (studentQuestion.TimeStarted == null)
                                {
                                    studentQuestion.TimeStarted = DateTime.Now;
                                    var mappedQuestion = _mapper.Map<StudentQuestionUpdateDto>(studentQuestion);
                                    await _studentQuestionService.UpdateAsync(mappedQuestion);
                                }

                                var model = _mapper.Map<StudentStudentQuestionDetailsVM>(studentQuestion);
                                model.QuestionCount = studentExam.StudentQuestions.Count;
                                // ExamDateTime'ı StartTime'dan al
                                model.ExamDateTime = studentExam.StartTime ?? DateTime.Now;
                                return View(model);
                            }
                            else
                                ViewBag.ErrorMessage = studentQuestionResult.Message;
                        }
                    }

                    studentExam.RetakeExam = false;
                    await _studentExamService.UpdateAsync(studentExam.Adapt<StudentExamUpdateDto>());
                }
            }
        }
        return RedirectToAction("StartExam", new { id = studentExamId });
    }

    /// <summary>
    /// Öğrencinin verdiği cevabı kaydeder ve sıradaki soruya ait bilgileri çekip öğrenciye gösterir.
    /// </summary>
    /// <param name="collection">Öğrencinin vermis olduğu cevabı collection["studentAnswer"] aracılığı ile taşır</param>
    /// <param name="model">Sonraki soruya ait bilgileri göstermek için gerekli bilgileri taşır</param>
    /// <returns>GetNextQuestion action'ını döner</returns>
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> GetNextQuestion(StudentStudentQuestionDetailsVM model, IFormCollection collection)
    {
        var studentExamResult = await _studentExamService.GetByIdAsync(model.StudentExamId);

        if (studentExamResult.IsSuccess)
        {
            var studentQuestionResult = await _studentQuestionService.GetByIdAsync(model.StudentQuestionId);

            if (studentQuestionResult.IsSuccess)
            {
                var studentExam = studentExamResult.Data;
                var examResult = await _examService.GetByIdAsync(studentExam.ExamId);

                if (examResult.IsSuccess)
                {
                    var exam = examResult.Data;

                    var mappedQuestion = _mapper.Map<StudentQuestionUpdateDto>(studentQuestionResult.Data);
                    mappedQuestion.TimeFinished = DateTime.Now;

                    if (!studentExam.RetakeExam) //Mazeretli sınavlar haricinde
                    {
                        //Öğrencinin sınavı tamamlayıp tamamlamadığını, sınavın başlayıp başlamadığını ve sınavın bitip bitmediğini denetler.
                        if (!studentExam.IsFinished && DateTime.Now >= exam.ExamDateTime && DateTime.Now <= exam.ExamDateTime + exam.ExamDuration)
                        {
                            var studentAnswersList = JsonSerializer.Deserialize<List<StudentAnswerCreateDto>>(collection["studentAnswers"]);
                            mappedQuestion.StudentAnswers = studentAnswersList;
                        }
                        await _studentQuestionService.UpdateAsync(mappedQuestion);

                        var mappedExam = _mapper.Map<StudentExamUpdateDto>(studentExam);
                        mappedExam.AnsweredQuestionCount++;
                        if (mappedExam.AnsweredQuestionCount >= model.QuestionCount || DateTime.Now >= exam.ExamDateTime + exam.ExamDuration)
                        {
                            mappedExam.IsFinished = true;
                            var studentExamScore = await CalculateStudentQuestionScoresAndExamScore(studentExam);
                            mappedExam.Score = studentExamScore > exam.MaxScore ? exam.MaxScore : studentExamScore;
                        }

                        await _studentExamService.UpdateAsync(mappedExam);

                    }
                    else
                    {
                        if (!studentExam.IsFinished && DateTime.Now >= studentExam.RetakeExamDate && DateTime.Now <= studentExam.RetakeExamDate + exam.ExamDuration)
                        {
                            var studentAnswersList = JsonSerializer.Deserialize<List<StudentAnswerCreateDto>>(collection["studentAnswers"]);
                            mappedQuestion.StudentAnswers = studentAnswersList;
                        }
                        await _studentQuestionService.UpdateAsync(mappedQuestion);
                        var retakeMappedExam = _mapper.Map<StudentExamUpdateDto>(studentExam);
                        retakeMappedExam.AnsweredQuestionCount++;
                        if (retakeMappedExam.AnsweredQuestionCount >= model.QuestionCount || DateTime.Now >= studentExam.RetakeExamDate + exam.ExamDuration)
                        {
                            retakeMappedExam.IsFinished = true;
                            var studentExamScore = await CalculateStudentQuestionScoresAndExamScore(studentExam);
                            retakeMappedExam.Score = studentExamScore > exam.MaxScore ? exam.MaxScore : studentExamScore;
                        }
                        await _studentExamService.UpdateAsync(retakeMappedExam);

                    }

                }
            }
        }
        return RedirectToAction("GetNextQuestion", new { studentExamId = model.StudentExamId });
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetExamResult(Guid studentExamId)
    {
        var studentExamResult = await _studentExamService.GetByIdAsync(studentExamId);

        if (studentExamResult.IsSuccess)
        {
            var studentExam = studentExamResult.Data;
            var examResult = await _examService.GetByIdAsync(studentExam.ExamId);


            if (examResult.IsSuccess)
            {
                var exam = examResult.Data;
                var model = _mapper.Map<StudentStudentExamReportVM>(studentExam);
                model = _mapper.Map(exam, model);

                if (studentExam.EndTime == null) // Öğrencinin sınavı bitiriş zamanını veritabanına ekle.
                {
                    var updateDto = _mapper.Map<StudentExamUpdateDto>(studentExam);
                    updateDto.EndTime = DateTime.Now;
                    await _studentExamService.UpdateAsync(updateDto);
                    studentExam.EndTime = DateTime.Now; // Memory'deki objeyi de güncelle
                }

                var studentResult = await _studentService.GetByIdAsync(studentExam.StudentId);

                //Öğrencinin emailini tutar.
                var studentEmail = await _studentExamService.GetStudentEmailByStudentExamAsync(studentExamId);



                if (studentResult.IsSuccess)
                {
                    model.StudentFullname = studentResult.Data.FirstName + " " + studentResult.Data.LastName;
                    TimeSpan totalTimeSpent = TimeSpan.Zero;
                    try
                    {
                        StudentExamResultDto performance = await _examAnalysisService.AnalysisStudentPerformanceAsync(studentExam.StudentId, studentExam.ExamId);

                        model.SubtopicPerformances = performance.Score;
                        model.SubtopicRightAnswers = performance.RightAnswer;
                        model.SubtopicWrongAnswers = performance.WrongAnswer;
                        model.SubtopicEmptyAnswers = performance.EmptyAnswer;

                        foreach (var item in model.StudentQuestions)
                        {
                            totalTimeSpent += (TimeSpan)(item.TimeFinished - item.TimeStarted);
                        }
                        model.TotalTimeSpend = totalTimeSpent;
                        model.FormattedTotalTimeSpend = model.TotalTimeSpend.ToString(@"hh\:mm\:ss");


                        string totalRightAnswers = model.SubtopicRightAnswers.Values.Sum().ToString();
                        string totalWrongAnswers = model.SubtopicWrongAnswers.Values.Sum().ToString();
                        string totalEmptyAnswers = model.SubtopicEmptyAnswers.Values.Sum().ToString();
                        //Değerler string formatına çevrilir.
                        string subtopicPerformances = FormatDictionary(model.SubtopicPerformances);
                        string subtopicRightAnswers = FormatDictionary(model.SubtopicRightAnswers);
                        string subtopicWrongAnswers = FormatDictionary(model.SubtopicWrongAnswers);
                        string subtopicEmptyAnswers = FormatDictionary(model.SubtopicEmptyAnswers);

                        //string listesine, mailde gözükecek olarak parametreleri ekler.
                        List<string> participantContents = new List<string>();

                        participantContents.Add($"{model.StudentFullname}" + "*?*" + $"{model.ExamName}" + "*?*" + $"{model.ExamDateTime}" + "*?*" + $"{model.Score}" + "*?*" + $"{model.QuestionCount}" + "*?*" + $"{totalRightAnswers}" + "*?*" + $"{totalWrongAnswers}" + "*?*" + $"{totalEmptyAnswers}");

                        //Sınav sonucu göndereceği mail adresi ve içeriği tutar.
                        var assessmentMailDto = new StudentAssesmentMailDto
                        {
                            StudentEmailAddress = studentEmail,
                            Result = participantContents
                        };

                        await _sendMailService.SendEmailToStudentAssessment(assessmentMailDto);

                    }
                    catch (InvalidOperationException ex)
                    {

                        ModelState.AddModelError(string.Empty, ex.Message);
                        return RedirectToAction("ErrorPage");
                    }

                }

                return View(model);
            }
        }

        //Soru bulunamazsa, sınav daha önce tamamlanmış ise, sınav süresi gelmemişse veya sınav süresi geçtiyse StartExam Ekranına yönlendirir.
        return RedirectToAction("StartExam", new { id = studentExamId });
    }

    /// <summary>
    /// Dictionary biçimlendirilir. Değerler string formatına çevrilir.
    /// </summary>
    /// <param name="dictionary"></param>
    /// <returns></returns>
    private string FormatDictionary<TKey, TValue>(Dictionary<TKey, TValue> dictionary)
    {
        return string.Join(" - ", dictionary.Select(kv => $"{kv.Value}"));
    }

    private async Task<decimal?> CalculateStudentQuestionScoresAndExamScore(StudentExamDto studentExam)
    {
        var studentQuestionsResult = await _studentQuestionService.GetByStudentExamIdAsync(studentExam.Id);
        studentExam.Score = 0;
        if (studentQuestionsResult.IsSuccess)
        {
            foreach (var item in studentQuestionsResult.Data)
            {
                var studentQuestionResult = await _studentQuestionService.GetByIdAsync(item.Id);
                if (studentQuestionResult.IsSuccess)
                {
                    var studentQuestion = studentQuestionResult.Data;

                    studentQuestion.Score = 0;

                    if (studentQuestion.StudentAnswers.Count > 0)
                    {
                        var answerIsCorrect = true;
                        foreach (var studentAnswer in studentQuestion.StudentAnswers)
                        {
                            var questionAnswerResult = await _questionAnswerService.GetById(studentAnswer.QuestionAnswerId);

                            if (questionAnswerResult.IsSuccess)
                            {
                                answerIsCorrect = studentAnswer.IsSelected == questionAnswerResult.Data.IsRightAnswer ? answerIsCorrect : false;
                            }
                            else
                                answerIsCorrect = false;
                        }
                        studentQuestion.Score = answerIsCorrect ? studentQuestion.MaxScore + studentQuestion.BonusScore : studentQuestion.Score;
                        studentExam.Score += studentQuestion.Score;
                        var mappedQuestion = _mapper.Map<StudentQuestionUpdateDto>(studentQuestion);
                        await _studentQuestionService.UpdateAsync(mappedQuestion);
                    }
                }
            }
        }
        return studentExam.Score;
    }

    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> MissExam([FromRoute] Guid id)
    {
        var studentExam = await _studentExamService.GetByIdAsync(id);
        var model = _mapper.Map<StudentStudentExamStartVM>(studentExam.Data);
        var exam = await _examService.GetByIdAsync(studentExam.Data.ExamId);
        model.ExamName = exam.Data.Name;
        model.ExamDateTime = exam.Data.ExamDateTime;
        model.ExamDuration = exam.Data.ExamDuration;
        model.EndExamTime = exam.Data.EndExamTime;
        if (model.ExcuseDescription == null)
            return View(model);
        else
        {
            var studentExamExcuse = _mapper.Map<StudentStudentExamExcuseVM>(model);
            return RedirectToAction("FeedBackPage", studentExamExcuse);
        }
    }


    /// <summary>
    /// This method updates the excuse description of a student who couldn't attend the exam.
    /// </summary>
    /// /// <param name="excuseDescription"> If no description is provided, it records the score as zero in the system. </param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> MissExam(StudentStudentExamStartVM studentStudentExamStartVM, Guid studentExamId)
    {
        var studentExamResult = await _studentExamService.GetByIdAsync(studentExamId);

        if (studentExamResult.IsSuccess)
        {
            var studentExam = studentExamResult.Data;
            if (studentStudentExamStartVM.ExcuseDescription != null)
            {
                studentExam.ExcuseDescription = studentStudentExamStartVM.ExcuseDescription;
                var mappedExam = _mapper.Map<StudentExamUpdateDto>(studentExam);
                await _studentExamService.UpdateAsync(mappedExam);
                var exam = await _examService.GetByIdAsync(studentExam.ExamId);
                studentStudentExamStartVM.ExamName = exam.Data.Name;
                studentStudentExamStartVM.ExamDateTime = exam.Data.ExamDateTime;
                studentStudentExamStartVM.ExamDuration = exam.Data.ExamDuration;
                studentStudentExamStartVM.EndExamTime = exam.Data.EndExamTime;
                var studentExamExcuse = _mapper.Map<StudentStudentExamExcuseVM>(studentStudentExamStartVM);
                return RedirectToAction("FeedBackPage", studentExamExcuse);
            }
            if (studentStudentExamStartVM.ExcuseDescription == null && studentExam.IsFinished == false)
            {
                studentExam.Score = 0;
                var mappedExam = _mapper.Map<StudentExamUpdateDto>(studentExam);
                await _studentExamService.UpdateAsync(mappedExam);
                var exam = await _examService.GetByIdAsync(studentExam.ExamId);
                studentStudentExamStartVM.ExamName = exam.Data.Name;
                studentStudentExamStartVM.ExamDateTime = exam.Data.ExamDateTime;
                studentStudentExamStartVM.ExamDuration = exam.Data.ExamDuration;
                studentStudentExamStartVM.EndExamTime = exam.Data.EndExamTime;
                var studentExamExcuse = _mapper.Map<StudentStudentExamExcuseVM>(studentStudentExamStartVM);
                return RedirectToAction("FeedBackPage", studentExamExcuse);
            }
        }
        return View();
    }


    /// <summary>
    /// Bu action mazeret geri bildirimi yapıldıktan sonra 
    /// </summary>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> FeedBackPage(StudentStudentExamExcuseVM studentStudentExamExcuseVM)
    {
        if (ModelState.IsValid)
            return View(studentStudentExamExcuseVM);
        else
            return BadRequest();
    }






}