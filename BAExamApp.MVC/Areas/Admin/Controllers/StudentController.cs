using AutoMapper;
using Microsoft.Extensions.Localization;
using BAExamApp.Business.Constants;
using BAExamApp.Business.Interfaces.Services;
using BAExamApp.Business.Services;
using BAExamApp.Dtos.Attributes;
using BAExamApp.Dtos.Classrooms;
using BAExamApp.Dtos.Emails;
using BAExamApp.Dtos.SendMails;
using BAExamApp.Dtos.Students;
using BAExamApp.Entities.DbSets;
using BAExamApp.MVC.Areas.Admin.Models.ExamVMs;
using BAExamApp.MVC.Areas.Admin.Models.SentMailVMs;
using BAExamApp.MVC.Areas.Admin.Models.StudentVMs;
using BAExamApp.MVC.Areas.Admin.Models.TrainerVMs;
using BAExamApp.MVC.Areas.Student.Models.StudentExamVMs;
using BAExamApp.MVC.Extensions;
using Hangfire;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Web.Helpers;
using X.PagedList;

namespace BAExamApp.MVC.Areas.Admin.Controllers;
[BreadcrumbName("Students")]
public class StudentController : AdminBaseController
{
    private readonly IStudentService _studentService;
    private readonly ISendMailService _sendMailService;
    private readonly IEmailService _emailService;
    private readonly IStudentClassroomService _studentClassroomService;
    private readonly IMapper _mapper;
    private readonly IStudentExamService _studentExamService;
    private readonly ITrainerClassroomService _trainerClassroomService;
    private readonly IClassroomService _classroomService;
    private readonly IExamService _examService;
    private readonly IExamAnalysisService _examAnalysisService;
    private readonly ISentMailService _sentMailService;


    public StudentController(IStudentService studentService, IMapper mapper, ISendMailService sendMailService, IEmailService emailService, IStudentClassroomService studentClassroomService, IStudentExamService studentExamService, ITrainerClassroomService trainerClassroomService, IClassroomService classroomService, IExamService examService, IExamAnalysisService examAnalysisService, ISentMailService sentMailService)
    {
        _studentService = studentService;
        _mapper = mapper;
        _sendMailService = sendMailService;
        _emailService = emailService;
        _studentClassroomService = studentClassroomService;
        _studentExamService = studentExamService;
        _trainerClassroomService = trainerClassroomService;
        _classroomService = classroomService;
        _examService = examService;
        _examAnalysisService = examAnalysisService;
        _sentMailService = sentMailService;
    }



    [HttpGet]
    public async Task<IActionResult> Index(int? page, int pageSize = 10, bool showAllActiveStudents = true)
    {
        ViewBag.Classrooms = await GetClassrooms();
        int pageNumber = page ?? 1;

        List<AdminStudentListVM> students = new List<AdminStudentListVM>();

        if (showAllActiveStudents)
        {
            var activeStudents = await _studentService.GetActiveStudentsAsync();
            students = _mapper.Map<List<AdminStudentListVM>>(activeStudents.Data);
        }
        var pagedList = students.ToPagedList(pageNumber, pageSize);

        ViewBag.PageSize = pageSize;
        ViewBag.PageNumber = pageNumber;
        ViewBag.ShowAllActiveStudents = showAllActiveStudents;

        return View(pagedList);
    }

 
    [HttpGet]
    public async Task<IActionResult> GetAllStudents()
    {
        var activeStudents = await _studentService.GetActiveStudentsAsync();
        var students = _mapper.Map<List<AdminStudentListVM>>(activeStudents.Data);

        return Json(students);
    }

    [HttpPost]
    public async Task<IActionResult> GetStudents(AdminStudentListVM adminStudentListVM, int? page, int pageSize = 10)
    {
        int pageNumber = page ?? 1;
        var getStudentResponse = await _studentService.GetStudentsByNameOrSurnameOrMailAdressAsync(
            adminStudentListVM.FirstName,
            adminStudentListVM.LastName,
            adminStudentListVM.Email
        );
        var studentList = _mapper.Map<List<AdminStudentListVM>>(getStudentResponse.Data);

        if (studentList is null || !studentList.Any())
        {
            ViewBag.NoResults = true;
        }
        var pagedList = studentList.ToPagedList(pageNumber, pageSize);
        ViewBag.PageSize = pageSize;

        return View("Index", pagedList);
    }

      

       

        



    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.Classroom = await GetClassrooms();
        return View(new AdminStudentCreateVM()
        {
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create(AdminStudentCreateVM studentCreateVM, IFormCollection collection)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                var errorMessage = error.ErrorMessage;
                NotifyErrorLocalized(errorMessage);
            }
            return RedirectToAction(nameof(Index));
        }
        var studentCreateDto = _mapper.Map<StudentCreateDto>(studentCreateVM);
        studentCreateDto.FirstName = StringExtensions.TitleFormat(studentCreateVM.FirstName);
        studentCreateDto.LastName = StringExtensions.TitleFormat(studentCreateVM.LastName);
        var addSutdentresult = await _studentService.AddAsync(studentCreateDto);
        if (!addSutdentresult.IsSuccess)
        {

            NotifyErrorLocalized(addSutdentresult.Message);
            return RedirectToAction(nameof(Index));
        }
        string link = Url.Action("index", "login", new { Area = "" }, Request.Scheme);
        BackgroundJob.Enqueue(() => _sendMailService.SendEmailNewStudent(new NewUserMailDto { Email = addSutdentresult.Data.Email, Url = link }));
        NotifySuccess($"{studentCreateVM.FirstName} {studentCreateVM.LastName} kişisi başarıyla eklendi, Mail Adresine bilgisi gönderildi.");
        var studentOtherEmailList = new List<EmailCreateDto>();
        var otherEmailList = collection["otherEmails"].ToList();
        var identityID = addSutdentresult.Data.IdentityId;
        foreach (var studentOtherEmail in otherEmailList)
        {
            studentOtherEmailList.Add(new EmailCreateDto() { EmailAddress = studentOtherEmail, IdentityId = identityID });
        }
        var addEmailResult = await _emailService.AddRangeAsync(studentOtherEmailList);
        if (addSutdentresult.IsSuccess)
        {
            NotifySuccessLocalized(addSutdentresult.Message);
            return RedirectToAction("Index");
        }
        else
        {
            NotifyErrorLocalized(addSutdentresult.Message);
            return RedirectToAction("Index");
        }
    }

    [HttpGet]
    public async Task<IActionResult> Update(Guid id)
    {
        ViewBag.Classroom = await GetClassrooms();
        var getStudentResult = await _studentService.GetByIdAsync(id);

        if (!getStudentResult.IsSuccess)
        {
            NotifyErrorLocalized(getStudentResult.Message);
            return RedirectToAction(nameof(Index));
        }
        var studentDto = getStudentResult.Data;
        var studentUpdateVM = _mapper.Map<AdminStudentUpdateVM>(studentDto);
        return PartialView("Update", studentUpdateVM);
    }

    [HttpPost]
    public async Task<IActionResult> Update(AdminStudentUpdateVM model, IFormCollection collection)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var updateStudent = _mapper.Map<StudentUpdateDto>(model);

        updateStudent.FirstName = StringExtensions.TitleFormat(model.FirstName);
        updateStudent.LastName = StringExtensions.TitleFormat(model.LastName);
        var updateStudentResult = await _studentService.UpdateAsync(updateStudent);

        if (!updateStudentResult.IsSuccess)
        {
            NotifyErrorLocalized(updateStudentResult.Message);
            return RedirectToAction(nameof(Update));
        }

        NotifySuccessLocalized(updateStudentResult.Message);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id, int? page, int pageSize = 10, bool showAllActiveStudents = true)
    {
        ViewBag.Classrooms = await GetClassrooms();
        var getStudent = await _studentService.GetStudentDetailsByIdAsync(id);
        if (getStudent.IsSuccess)
        {
            var studentDetailsVM = _mapper.Map<AdminStudentDetailVM>(getStudent.Data);

            // Sınavları al ve viewmodel'e ekle
            var getStudentExams = await _studentExamService.GetAllExamsWithDetailsByIdAsync(id);
            if (getStudentExams.IsSuccess)
            {
                studentDetailsVM.StudentExams = _mapper.Map<IEnumerable<StudentExamsForAdminVM>>(getStudentExams.Data);
            }

            // Diğer kodlar...
            studentDetailsVM.FirstName = ConvertFirstLetterToUpperCase(studentDetailsVM.FirstName);
            studentDetailsVM.LastName = ConvertFirstLetterToUpperCase(studentDetailsVM.LastName);

            studentDetailsVM.Classrooms = (await _studentClassroomService.GetStudetClassroomIdentityIdAsync(getStudent.Data.Id)).Data;
            var classrooms = studentDetailsVM.Classrooms.Select(x => ConvertFirstLetterToUpperCase(x)).ToList();
            studentDetailsVM.Classrooms = classrooms;

            ViewBag.PageNumber = page ?? 1;
            ViewBag.PageSize = pageSize;
            ViewBag.ShowAllActiveStudents = showAllActiveStudents;

            return View(studentDetailsVM);
        }

        NotifyErrorLocalized(getStudent.Message);
        return RedirectToAction(nameof(Index));
    }


    private static string ConvertFirstLetterToUpperCase(string text)
    {
        string[] words = text.Split(' ');

        for (int i = 0; i < words.Length; i++)
        {
            if (!string.IsNullOrEmpty(words[i]))
                words[i] = char.ToUpper(words[i][0]) + words[i].Substring(1).ToLower();
        }
        return string.Join(" ", words);
    }


    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleteResult = await _studentService.DeleteAsync(id);
        if (deleteResult.IsSuccess)
            NotifySuccessLocalized(deleteResult.Message);
        else
            NotifyErrorLocalized(deleteResult.Message);

        return Json(deleteResult);

    }



    [HttpGet]
    public async Task<IActionResult> StudentExams(Guid id)
    {
        var getStudentExams = await _studentExamService.GetAllExamsWithDetailsByIdAsync(id);

        if (getStudentExams.IsSuccess)
        {
            var studentExamsVM = _mapper.Map<IEnumerable<StudentExamsForAdminVM>>(getStudentExams.Data);

            return View(studentExamsVM);
        }
        NotifyErrorLocalized(getStudentExams.Message);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> StudentSentMails(string email, int? page, int pageSize = 10, bool notifyShow = true, string mailStatus = null, string sendingDate = null)
    {
        int pageNumber = page ?? 1;

        var getStudentSentMails = await _sentMailService.GetAllSentMailWithDetailsByEmailAsync(email);

        if (getStudentSentMails.IsSuccess)
        {
            var studentSentMailVM = _mapper.Map<IEnumerable<AdminStudentMailListVM>>(getStudentSentMails.Data);

            // **Filtreleme yapılıyor**
            studentSentMailVM = FilterSentMails(studentSentMailVM, mailStatus, sendingDate);

            // **Öğrenci bilgilerini sakla**
            var studentInfo = getStudentSentMails.Data.FirstOrDefault();
            if (studentInfo != null)
            {
                ViewBag.StudentFullName = studentInfo.StudentFullName;
                ViewBag.StudentEmail = studentInfo.Email;
                ViewBag.LatestClassroom = studentInfo.LatestClassroom;
                ViewBag.LatestTrainers = studentInfo.LatestClassroomsTrainers;
            }

            // **Eğer filtreleme sonrası veri yoksa, boş model döndür ama öğrenci bilgilerini koru**
            var pagedList = studentSentMailVM.Any() ? studentSentMailVM.ToPagedList(pageNumber, pageSize) : new List<AdminStudentMailListVM>().ToPagedList(1, pageSize);

            ViewBag.PageSize = pageSize;
            ViewBag.PageNumber = pageNumber;

            if (notifyShow)
            {
                NotifySuccessLocalized(getStudentSentMails.Message);
            }

            return View(pagedList);
        }

        if (notifyShow)
        {
            NotifyErrorLocalized(getStudentSentMails.Message);
        }

        return RedirectToAction(nameof(Index));
    }


    [HttpPost]
    public async Task<IActionResult> ResendStudentEmail(Guid sentMailId)
    {
        string studentEmail = null;
        try
        {
            if (sentMailId != Guid.Empty)
            {
                studentEmail = await _sendMailService.GetStudentEmailById(sentMailId);
                var sentMail = await _sendMailService.GetSentMail(sentMailId);

                //Use Hangfire to enqueue the email sending task
                BackgroundJob.Enqueue(() => _sendMailService.ResendMail(sentMail));
                NotifySuccessLocalized(Messages.EmailSendSuccess);
                return RedirectToAction(nameof(StudentSentMails), new { email = studentEmail, notifyShow = false });
            }
            NotifyErrorLocalized(Messages.EmailSendError);
            return RedirectToAction(nameof(StudentSentMails), new { email = studentEmail, notifyShow = false });

        }
        catch (Exception)
        {
            NotifyErrorLocalized(Messages.EmailSendError);
            return RedirectToAction(nameof(StudentSentMails), new { email = studentEmail, notifyShow = false });
        }
    }


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
                TimeSpan examDuration = examResult.Data.ExamDuration;
                var model = _mapper.Map<StudentStudentExamReportVM>(studentExam);
                model = _mapper.Map(exam, model);

                var studentResult = await _studentService.GetByIdAsync(studentExam.StudentId);
                if (studentResult.IsSuccess)
                {
                    model.StudentFullname = studentResult.Data.FirstName + " " + studentResult.Data.LastName;
                    try
                    {
                        var performance = await _examAnalysisService.AnalysisStudentPerformanceAsync(studentExam.StudentId, studentExam.ExamId);
                        model.SubtopicPerformances = performance.Score;
                        model.SubtopicRightAnswers = performance.RightAnswer;
                        model.SubtopicWrongAnswers = performance.WrongAnswer;
                        model.SubtopicEmptyAnswers = performance.EmptyAnswer;

                        model.FormattedTotalTimeSpend = model.TotalTimeSpend.ToString(@"hh\:mm\:ss");

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
        return RedirectToAction("StudentExams", new { id = studentExamResult.Data.ExamId });
    }

    [HttpGet]
    public async Task<IActionResult> StudentExamDetails(Guid id)
    {
        var getStudentExam = await _studentExamService.GetExamStrudentQuestionDetailsByIdAsync(id);
        if (getStudentExam.IsSuccess)
        {
            var studentExamDetailVM = _mapper.Map<AdminExamStudentQuestionDetailsVM>(getStudentExam.Data);
            return View(studentExamDetailVM);
        }
        NotifyErrorLocalized(getStudentExam.Message);
        return RedirectToAction(nameof(Index));
    }

    public async Task<AdminStudentUpdateVM> GetStudent(Guid studentId)
    {
        var studentFoundResult = await _studentService.GetByIdAsync(studentId);
        var studentUpdateVM = _mapper.Map<AdminStudentUpdateVM>(studentFoundResult.Data);

        var classrooms = await _studentClassroomService.GetAllByStudentIdAsync(studentId);
        studentUpdateVM.ClassroomIds = classrooms.Data.Select(c => c.ClassroomId).ToList();


        studentUpdateVM.ClassroomSelectList = await GetClassrooms();
        return studentUpdateVM;
    }


    private async Task<SelectList> GetClassrooms(Guid? classroomId = null)
    {
        var clasroomList = (await _classroomService.GetActiveClassroomsAsync()).Data;
        return new SelectList(clasroomList.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = x.Name,
            Selected = x.Id == (classroomId != null ? classroomId.Value : classroomId)
        }), "Value", "Text");

    }

    /// <summary>
    /// Filtreleme işlemi uygulayarak gönderilen maillerin listesini döner.
    /// Verilen mail durumu (mailStatus) ve gönderim tarihi (sendingDate) parametrelerine göre filtreleme yapar.
    /// </summary>
    /// <param name="mails">Filtreleme işlemi yapılacak mail listesi.</param>
    /// <param name="mailStatus">Mailin başarılı olup olmadığını belirten bir değer (true/false).</param>
    /// <param name="sendingDate">Mailin gönderildiği tarih. (ModifiedDate)</param>
    /// <returns>Filtrelenmiş mail listesi.</returns>
    private IEnumerable<AdminStudentMailListVM> FilterSentMails(IEnumerable<AdminStudentMailListVM> mails, string mailStatus, string sendingDate)
    {
        if (!string.IsNullOrEmpty(mailStatus) && bool.TryParse(mailStatus, out bool isMailSuccess))
        {
            mails = mails.Where(x => x.IsSuccess == isMailSuccess);
        }

        if (!string.IsNullOrEmpty(sendingDate) && DateTime.TryParse(sendingDate, out DateTime sendingDateTime))
        {
            mails = mails.Where(x => x.ModifiedDate?.Date == sendingDateTime);
        }

        return mails;
    }

    [HttpGet]
    public async Task<IActionResult> SearchStudentFirstNameAutocomplete(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return Json(new List<object>());

        var students = await _studentService.GetAllAsync();

        var matched = students.Data
            .Where(x => x.FirstName.Contains(term, StringComparison.OrdinalIgnoreCase))
            .Select(x => x.FirstName)
            .Distinct()
            .Take(10)
            .ToList();

        return Json(matched);
    }

    [HttpGet]
    public async Task<IActionResult> SearchStudentLastNameAutocomplete(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return Json(new List<string>());

        var students = await _studentService.GetAllAsync();

        var matched = students.Data
            .Where(x => x.LastName.Contains(term, StringComparison.OrdinalIgnoreCase))
            .Select(x => x.LastName)
            .Distinct()
            .Take(10)
            .ToList();

        return Json(matched);
    }

    [HttpGet]
    public async Task<IActionResult> SearchStudentEmailAutocomplete(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return Json(new List<string>());

        var students = await _studentService.GetAllAsync();

        var matched = students.Data
            .Where(x => x.Email.Contains(term, StringComparison.OrdinalIgnoreCase))
            .Select(x => x.Email)
            .Distinct()
            .Take(10)
            .ToList();

        return Json(matched);
    }


}
