using AutoMapper;
using BAExamApp.Core.Enums;
using BAExamApp.Dtos.Attributes;
using BAExamApp.Dtos.TecnicalUnits;
using BAExamApp.MVC.Areas.Admin.Models.TechnicalUnitVMs;
using X.PagedList;

namespace BAExamApp.MVC.Areas.Admin.Controllers;
[BreadcrumbName("TechnicalUnits")]
public class TechnicalUnitController : AdminBaseController
{
    private readonly ITechnicalUnitService _technicalUnitService;
    private readonly IMapper _mapper;
    public TechnicalUnitController(ITechnicalUnitService technicalUnitService, IMapper mapper)
    {
        _technicalUnitService = technicalUnitService;
        _mapper = mapper;
    }
    
    [HttpGet]
    public async Task<IActionResult> Index(string typeOfTechnicalUnit, int? page, int pageSize = 10)
    {
        int pageNumber = page ?? 1;

        var technicalUnitGetResult = await _technicalUnitService.GetAllAsync();
        var technicalUnitList = _mapper.Map<List<AdminTechnicalUnitListVM>>(technicalUnitGetResult.Data).OrderBy(x => x.Name).ToList();

        if (!string.IsNullOrEmpty(typeOfTechnicalUnit))
            technicalUnitList = await Search(typeOfTechnicalUnit);
        
        var pagedList = technicalUnitList.ToPagedList(pageNumber, pageSize);
        ViewBag.PageSize = pageSize;
        ViewBag.TypeOfTechnicalUnit = typeOfTechnicalUnit;

        return View(pagedList);
    }
    
    public async Task<List<AdminTechnicalUnitListVM>> Search(string technicalUnitName)
    {
        var technicalUnitGetResult = await _technicalUnitService.GetAllAsync();
        var technicalUnitList = _mapper.Map<List<AdminTechnicalUnitListVM>>(technicalUnitGetResult.Data);

        var searchList = technicalUnitList
            .Where(s => s.Name.IndexOf(technicalUnitName, StringComparison.OrdinalIgnoreCase) >= 0)
            .OrderBy(o => o.Name)
            .ToList();

        return searchList;
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(AdminTechnicalUnitCreateVM technicalUnitCreateVM)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(x => x.Errors);
            string errorMessages = null!;
            foreach (var error in errors)
            {
                errorMessages += " ," + error.ErrorMessage;
            }
            NotifyError(errorMessages);
            return RedirectToAction(nameof(Index));
        }

        var addResult = await _technicalUnitService.AddAsync(_mapper.Map<TechnicalUnitCreateDto>(technicalUnitCreateVM));
        if (!addResult.IsSuccess)
        {
            NotifyErrorLocalized(addResult.Message);
            return RedirectToAction(nameof(Index));
        }

        NotifySuccessLocalized(addResult.Message);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Update(AdminTechnicalUnitUpdateVM technicalUnitUpdateVM)
    {
        if (!ModelState.IsValid)
        {
            return View(technicalUnitUpdateVM);
        }

        var technicalUnitUpdateDto = _mapper.Map<TechnicalUnitUpdateDto>(technicalUnitUpdateVM);
        var updateResult = await _technicalUnitService.UpdateAsync(technicalUnitUpdateDto);
        if (!updateResult.IsSuccess)
        {
            NotifyErrorLocalized(updateResult.Message);
            return View(technicalUnitUpdateVM);
        }

        NotifySuccessLocalized(updateResult.Message);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        var tecnicalUnitDeleteResponse = await _technicalUnitService.DeleteAsync(id);
        if (tecnicalUnitDeleteResponse.IsSuccess)
            NotifySuccessLocalized(tecnicalUnitDeleteResponse.Message);
        else
            NotifyErrorLocalized(tecnicalUnitDeleteResponse.Message);

        return Json(tecnicalUnitDeleteResponse);
    }

    [HttpGet]
    public async Task<IActionResult> SearchTechnicalUnitAutocomplete(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return Json(new List<object>());

        var technicalUnits = await _technicalUnitService.GetAllAsync();

        var matched = technicalUnits.Data
            .Where(x => x.Name.Contains(term, StringComparison.OrdinalIgnoreCase))
            .Select(x => x.Name)
            .Distinct()
            .Take(10)
            .ToList();

        return Json(matched);
    }


}