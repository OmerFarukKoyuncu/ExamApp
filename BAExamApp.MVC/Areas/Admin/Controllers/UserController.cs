using AutoMapper;
using BAExamApp.Core.Enums;
using BAExamApp.Dtos.Trainers;
using BAExamApp.Dtos.Users;
using BAExamApp.MVC.Areas.Admin.Models.TrainerVMs;
using BAExamApp.MVC.Areas.Admin.Models.UsersVMs;
using Microsoft.AspNetCore.Identity;
using X.PagedList;

namespace BAExamApp.MVC.Areas.Admin.Controllers;
public class UserController : AdminBaseController
{
    private readonly IUserService _userService;
    private readonly IRoleService _roleService;
    private readonly ITrainerService _trainerService;
    private readonly IAccountService _accountService;
    private readonly ITechnicalUnitService _technicalUnitService;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly IMapper _mapper;

    public UserController(IMapper mapper, IUserService userService, IRoleService roleService, ITrainerService trainerService, IAccountService accountService, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ITechnicalUnitService technicalUnitService)
    {
        _mapper = mapper;
        _userService = userService;
        _roleService = roleService;
        _trainerService = trainerService;
        _accountService = accountService;
        _userManager = userManager;
        _signInManager = signInManager;
        _technicalUnitService = technicalUnitService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string trainerName, int? page, int pageSize = 10, bool? showAllData = null, bool showCreateModal = false,string? Role="Trainer")
    {
        if (showAllData == null && HttpContext.Session.GetInt32("ShowAllData") != null)
        {
            showAllData = HttpContext.Session.GetInt32("ShowAllData") == 1;
        }

        bool showAll = showAllData ?? false;

        HttpContext.Session.SetInt32("ShowAllData", showAll ? 1 : 0);


        ViewBag.TecnicalUnit = await GetTechnicalUnitsAsync();


        int pageNumber = page ?? 1;
        ViewBag.PageSize = pageSize;
        ViewBag.CurrentPage = pageNumber;

        var result = await _trainerService.GetAllWithClassroomCountsAsync();
        var trainerList = _mapper.Map<IEnumerable<AdminTrainerListVM>>(result.Data).OrderBy(x => x.FirstName).ThenBy(x => x.LastName).ToList();



        if (!string.IsNullOrEmpty(trainerName))
        {
            trainerList = await GetTrainerFromTable(trainerName);


        }
        if (!showAll)
        {
            trainerList = trainerList.Where(trainer => trainer.Status == Status.Active).ToList();
        }

        foreach (var trainer in trainerList)
        {
            trainer.FirstName = ToPascalCase(trainer.FirstName);
            trainer.LastName = ToPascalCase(trainer.LastName);
        }

        trainerList = trainerList.OrderBy(trainer => trainer.ModifiedDate).ToList();
        ViewBag.ShowAllData = showAll;
        ViewBag.ShowCreateModal = showCreateModal;
        var paginatedList = trainerList.ToPagedList(pageNumber, pageSize);
        ViewBag.TrainerName = trainerName;


        return View(paginatedList);

    }

    [HttpPost]
    public async Task<IActionResult> UpdateUserRoles(List<UserListVM> model)
    {
        var result = await _roleService.UpdateUserRole(_mapper.Map<List<UserRoleAssingDto>>(model), "1");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> UpdateUserRole(AdminUserRoleUpdateVM adminUserRoleUpdateVM)
    {
        var result = await _roleService.ChangeUserRole(_mapper.Map<UserRoleUpdateDto>(adminUserRoleUpdateVM));
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> LoginAsTrainer(Guid trainerId)
    {
        var selectedTrainer = await _trainerService.GetByIdAsync(trainerId);
        var infoOfTrainer = await _userManager.FindByIdAsync(selectedTrainer.Data.IdentityId);
        var adminId = (await _userManager.FindByNameAsync(User.Identity!.Name!))!.Id;
        HttpContext.Session.SetString("changeSession", "true");
        HttpContext.Session.SetString("adminId", adminId);
        await _signInManager.SignInAsync(infoOfTrainer!, isPersistent: false);
        return RedirectToAction("Index", "Home", new { area = "Trainer" });

    }

    public async Task<List<AdminTrainerListVM>> GetTrainerFromTable(string trainerName)
    {
        var getTrainers = await _trainerService.GetAllAsync();
        var trainers = getTrainers.Data.Adapt<List<AdminTrainerListVM>>();

        var trainerList = trainers.Where(x => x.FirstName.IndexOf(trainerName, StringComparison.OrdinalIgnoreCase) >= 0 || x.LastName.IndexOf(trainerName, StringComparison.OrdinalIgnoreCase) >= 0).ToList()
                              .OrderBy(x => x.FirstName).ToList();
        return trainerList;
    }

    private async Task<SelectList> GetTechnicalUnitsAsync(Guid? technicalUnitId = null)
    {
        var technicalUnitList = (await _technicalUnitService.GetAllAsync()).Data;
        return new SelectList(technicalUnitList.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = x.Name,
            Selected = x.Id == (technicalUnitId != null ? technicalUnitId.Value : technicalUnitId)
        }), "Value", "Text");

    }

    private string ToPascalCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;
        return string.Join(" ", input.Split(' ')
                                     .Where(w => !string.IsNullOrEmpty(w))
                                     .Select(w => char.ToUpper(w[0]) + w.Substring(1).ToLower()));
    }
}
