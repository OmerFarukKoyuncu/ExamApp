using AutoMapper;
using BAExamApp.Dtos.Attributes;
using BAExamApp.Dtos.BreadcrumbItem;
using BAExamApp.Dtos.QuestionDifficulty;
using BAExamApp.Entities.DbSets;
using BAExamApp.Entities.Enums;
using BAExamApp.MVC.Areas.Admin.Models.QuestionDifficultyVMs;
using BAExamApp.MVC.Areas.Admin.Models.QuestionVMs;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using X.PagedList;

namespace BAExamApp.MVC.Areas.Admin.Controllers;

[BreadcrumbName("Question", false)]
public class QuestionsDifficultyController : AdminBaseController
{

    private readonly IQuestionDifficultyService _questionDifficultyService;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _memoryCache;
    private readonly IBreadcrumbService _breadcrumbService;
    private readonly IStringLocalizer<SharedModelResource> _stringLocalizer;

    public QuestionsDifficultyController(IQuestionDifficultyService questionDifficultyService, IMapper mapper, IMemoryCache memoryCache, IBreadcrumbService breadcrumbService, IStringLocalizer<SharedModelResource> stringLocalizer)
    {
        _questionDifficultyService = questionDifficultyService;
        _mapper = mapper;
        _memoryCache = memoryCache;
        _breadcrumbService = breadcrumbService;
        _stringLocalizer = stringLocalizer;
    }

    
    [HttpGet]
    public async Task<IActionResult> Index(State state, int? page, int pageSize = 10)
    {
        //Breadcrumb'ı manuel moda geçir.
        _breadcrumbService.SwitchToManual();
        _breadcrumbService.AddBreadcrumb(new BreadcrumbItemDto
        {
            Title = "Question",
        });
        
        //İLgili duruma göre breadcrumb hazırla.
        _breadcrumbService.AddBreadcrumb(new BreadcrumbItemDto
        {
            Title = _stringLocalizer["QuesitionDifficulty_Name"],
            Url = $"/Question/QuestionsDifficulty",
            IsActive = true
        });
        
        int pageNumber = page ?? 1;

        var questionDifficulty = await _questionDifficultyService.GetAllAsync();
        var questionDifficultyList = _mapper.Map<List<AdminQuestionDifficultyListVM>>(questionDifficulty.Data);
        var pagedList = questionDifficultyList.ToPagedList(pageNumber, pageSize);

     
        ViewBag.PageSize = pageSize;
        ViewData["QuestionDifficulty_State"] = (int)state;

        return View(pagedList);
    }


    [HttpPost]
    public async Task<IActionResult> Create(AdminQuestionDifficultyCreateVM model)
    {
        if (!ModelState.IsValid)
        {
            var erros = ModelState.Values.SelectMany(x => x.Errors);
            string errorMessages = null!;
            foreach (var error in erros)
            {
                errorMessages += ", " + error.ErrorMessage;
            }
            NotifyError(errorMessages);
            return RedirectToAction(nameof(Index));
        }

        var questionDificulty = _mapper.Map<QuestionDifficultyCreateDto>(model);
        var result = await _questionDifficultyService.AddAsync(questionDificulty);
        if (result.IsSuccess)
        {

            NotifySuccess(Localize(result.Message));
            return RedirectToAction(nameof(Index));
        }

        NotifyErrorLocalized(result.Message);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var questionDifficulty = await _questionDifficultyService.GetDetailsByIdAsync(id);
        if (questionDifficulty.IsSuccess)
        {
            var questionDifficultyVm = _mapper.Map<AdminQuestionDifficultyDetailVM>(questionDifficulty.Data);
            return View(questionDifficultyVm);
        }
        NotifyErrorLocalized(questionDifficulty.Message);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Update(Guid id)
    {
        var questionDifficultyResult = await _questionDifficultyService.GetDetailsByIdAsync(id);
        if (!questionDifficultyResult.IsSuccess)
        {
            NotifyErrorLocalized(questionDifficultyResult.Message);
            return RedirectToAction(nameof(Index));
        }
        var updateQuestionDifficultyResult = _mapper.Map<AdminQuestionDifficultyUpdateVM>(questionDifficultyResult.Data);
        return PartialView("Update", updateQuestionDifficultyResult);
    }


    [HttpPost]
    public async Task<IActionResult> Update(AdminQuestionDifficultyUpdateVM model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var updateQuestionDifficultyDto = _mapper.Map<QuestionDifficultyUpdateDto>(model);
        var updateQuestionDifficultyResult = await _questionDifficultyService.UpdateAsync(updateQuestionDifficultyDto);
        if (!updateQuestionDifficultyResult.IsSuccess)
        {
            NotifyErrorLocalized(updateQuestionDifficultyResult.Message);
            return View(model);
        }
        NotifySuccessLocalized(updateQuestionDifficultyResult.Message);
        return RedirectToAction(nameof(Index));

    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        var questionDifficultyResponse = await _questionDifficultyService.DeleteAsync(id);
        if (questionDifficultyResponse.IsSuccess)
        {
            NotifySuccessLocalized(questionDifficultyResponse.Message);
        }
        else
        {
            NotifyErrorLocalized(questionDifficultyResponse.Message);
        }
        return Json(questionDifficultyResponse);
    }

    [HttpGet]
    public async Task<IActionResult> GetQuestionsByGivenValues(int? page = 1, int pageSize = 10)
    {
        _memoryCache.TryGetValue("filteredQuestionList", out List<AdminQuestionListVM> questionList);
        _memoryCache.TryGetValue("state", out var _state);
        _memoryCache.TryGetValue("subjectList", out var subjectList);
        _memoryCache.TryGetValue("SubtopicList", out var subtopicList);
        _memoryCache.TryGetValue("questionDifficultyList", out var questionDifficultyList);

        int pageNumber = page ?? 1;

        if (questionList != null && _state != null)
        {
            // IPagedList türünde sayfalama yapısı oluştur
            var pagedList = questionList.ToPagedList(pageNumber, pageSize);

            ViewData["Question_State"] = _state;
            ViewBag.SubjectList = subjectList;
            ViewBag.SubtopicList = subtopicList;
            ViewBag.QuestionDifficultyList = questionDifficultyList;
            ViewBag.PageSize = pageSize;

            return View("Index", pagedList);
        }
        // Eğer cache'de veri yoksa QuestionList'e yönlendir
        return RedirectToAction("Index", "QuestionsDifficulty",
            new { state = _state, page = pageNumber, pageSize, showAllQuestions = true });
        
    }

    [HttpGet]
    public async Task<IActionResult> SearchQuestionDifficultyTypeAutocomplete(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return Json(new List<object>());

        var questions = await _questionDifficultyService.GetAllAsync();

        var matched = questions.Data
            .Where(x => x.Name.Contains(term, StringComparison.OrdinalIgnoreCase))
            .Select(x => x.Name)
            .Distinct()
            .Take(10)
            .ToList();

        return Json(matched);
    }
}
