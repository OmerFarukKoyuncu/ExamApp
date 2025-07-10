using AutoMapper;
using BAExamApp.Dtos.Attributes;
using BAExamApp.MVC.Areas.Trainer.Models.ClassroomVMs;
using X.PagedList;

namespace BAExamApp.MVC.Areas.Trainer.Controllers;

[BreadcrumbName("Classroom_List")]
public class ClassroomController : TrainerBaseController
{
    private readonly ITrainerService _trainerService;
    private readonly IClassroomService _classroomService;
    private readonly IMapper _mapper;
    private readonly IStudentExamService _studentExamService;
    private readonly IExamService _examService;

    public ClassroomController(ITrainerService trainerService, IClassroomService classroomService, IMapper mapper, IExamService examService, IExamAnalysisService examAnalysisService, IStudentExamService studentExamService)
    {
        _trainerService = trainerService;
        _classroomService = classroomService;
        _mapper = mapper;
        _studentExamService = studentExamService;
        _examService = examService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(State state, int? page, int pageSize = 10, bool showAllQuestions = false, string searchQuery = null)
    {
        var classroomResult = await _trainerService.GetClassroomsByIdentityId(UserIdentityId);

        var classrooms = _mapper.Map<IEnumerable<TrainerClassroomListVM>>(classroomResult.Data);

        // Arama filtresi varsa uygula (örnek)
        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            classrooms = classrooms.Where(c => c.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase));
        }

        int pageNumber = page ?? 1;

        var pagedList = classrooms.ToPagedList(pageNumber, pageSize);

        ViewBag.PageSize = pageSize;
        ViewBag.SearchQuery = searchQuery;

        return View(pagedList);
    }


    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        var classroomDetailResult = await _classroomService.GetDetailsByIdAsync(id);

        if (!classroomDetailResult.IsSuccess)
            return NotFound();

        var classroomDetails = _mapper.Map<TrainerClassroomDetailsVM>(classroomDetailResult.Data);

        return View(classroomDetails);
    }

    public async Task<IActionResult> ExamAnalysis(Guid examId, string classroomName)
    {
        var user = (await _trainerService.GetByIdentityIdAsync(UserIdentityId)).Data;

        var trainerId = user.Id;
         
        var examAnalysis = await _studentExamService.AnalysisExamPerformanceByTrainerAsync(trainerId, examId);

        var model = new TrainerClassroomExamAnalysisVM
        {
            SubtopicPerformances = examAnalysis
        };

        var exam = await _examService.GetByIdAsync(examId);
        ViewBag.ExamName = exam.Data.Name;

        ViewBag.ClassroomName = classroomName;


        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> SearchClassroomNameAutocomplete(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return Json(new List<string>());

        var apiUsers = await _classroomService.GetClassroomsForAutoComplete(term);

        var matched = apiUsers.Data.Select(x => x.Name);

        return Json(matched);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllClassrooms()
    {
        var apiUsers = await _classroomService.GetAllForIndexAsync();

        var matched = apiUsers.Data.Select(x => x.Name);

        return Json(matched);
    }
}