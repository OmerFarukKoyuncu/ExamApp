using AutoMapper;
using BAExamApp.BackgroundJobs.Schedules;
using BAExamApp.Business.Constants;
using BAExamApp.Core.Enums;
using BAExamApp.Core.Utilities.Results;
using BAExamApp.Dtos.Admins;
using BAExamApp.Dtos.Attributes;
using BAExamApp.Dtos.Emails;
using BAExamApp.Dtos.SendMails;
using BAExamApp.Dtos.Trainers;
using BAExamApp.Dtos.Users;
using BAExamApp.Entities.DbSets;
using BAExamApp.MVC.Areas.Admin.Models.AdminVMs;
using BAExamApp.MVC.Extensions;
using Hangfire;
using Mapster.Utils;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Localization;
using System.Globalization;
using X.PagedList;


namespace BAExamApp.MVC.Areas.Admin.Controllers;
[BreadcrumbName("Admins")]
public class AdminController : AdminBaseController
{
    private readonly IAdminService _adminService;
    private readonly ISendMailService _sendMailService;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;
    private readonly IRoleService _roleService;
    private readonly ITrainerService _trainerService;
    private readonly ITechnicalUnitService _technicalUnitService;
    private readonly IUserService _userService;
    private readonly IProductService _productService;
    private readonly IBranchService _branchService;
    private readonly IStringLocalizer<SharedModelResource> _stringLocalizer;
    public AdminController(IAdminService adminService, IMapper mapper, ISendMailService sendMailService, IEmailService emailService, IRoleService roleService, ITrainerService trainerService,ITechnicalUnitService technicalUnitService,IUserService userService, IStringLocalizer<SharedModelResource> stringLocalizer, IProductService productService, IBranchService branchService)
    {
        _adminService = adminService;
        _mapper = mapper;
        _sendMailService = sendMailService;
        _emailService = emailService;
        _roleService = roleService;
        _trainerService = trainerService;
        _technicalUnitService= technicalUnitService;
        _userService = userService;
        _productService=productService;
        _branchService=branchService;
        _stringLocalizer = stringLocalizer;

    }

    /// <summary> 
    /// Bu yöntem, admin listesini filtreleme ve sayfalama işlemlerini gerçekleştirir. 
    /// Tüm adminleri alır, verilen filtreleme kriterlerine (firstName, lastName, email) göre filtreler 
    /// ve sayfalara bölünmüş bir admin listesi döndürür. Filtreleme kriterleri ve sayfalama detayları, 
    /// ViewBag kullanılarak view'e aktarılır. 
    /// </summary>
    
    public async Task<IActionResult> Index(string firstName, string lastName, string email, int? page, int pageSize = 10)
    {
        
        int pageNumber = page ?? 1;
        //int pageSize = 10;

        var result = await _adminService.GetAllAsync();
        var adminList = _mapper.Map<List<AdminAdminListVM>>(result.Data).OrderBy(x=>x.FirstName).ThenBy(x=>x.LastName).ToList();
        if (!string.IsNullOrEmpty(firstName))
        {
            firstName = firstName.Trim();
            adminList = adminList.Where(a=>a.FirstName.Contains(firstName, StringComparison.CurrentCultureIgnoreCase)).ToList();
        }

        if (!string.IsNullOrEmpty(lastName))
        {
            lastName = lastName.Trim();
            adminList = adminList.Where(a => a.LastName.Contains(lastName, StringComparison.CurrentCultureIgnoreCase)).ToList();
        }

        if (!string.IsNullOrEmpty(email))
        {
            email = email.Trim();
            adminList = adminList.Where(a => a.Email.Contains(email, StringComparison.CurrentCultureIgnoreCase)).ToList();
        }

        var paginatedList = adminList.ToPagedList(pageNumber, pageSize);

        ViewBag.PageSize = pageSize;
        ViewBag.CurrentPage = pageNumber;
        ViewBag.FirstName = firstName;
        ViewBag.LastName = lastName;
        ViewBag.Email = email;

        return View(paginatedList);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        return View(new AdminAdminCreateVM()
        {
        });
    }

    [HttpPost]
    public async Task<IActionResult> Create(AdminAdminCreateVM model, IFormCollection collection)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                var errorMessage = error.ErrorMessage;
                //var propertyName = error.PropertyName;
            }
            return RedirectToAction(nameof(Index));
        }

        var adminDto = _mapper.Map<AdminCreateDto>(model);

        adminDto.FirstName = StringExtensions.TitleFormat(model.FirstName);
        adminDto.LastName = StringExtensions.TitleFormat(model.LastName);

        var addAdminResult = await _adminService.AddAsync(adminDto);
        if (!addAdminResult.IsSuccess)
        {
            NotifyErrorLocalized(_stringLocalizer["Turkish_Characters_Not_Allowed"]);
            return RedirectToAction(nameof(Index));
        }

        var adminOtherEmailList = new List<EmailCreateDto>();
        var otherEmailsList = collection["otherEmails"].ToList();
        var identityId = addAdminResult.Data.IdentityId;

        foreach (var adminOtherEmail in otherEmailsList)
        {
            adminOtherEmailList.Add(new EmailCreateDto() { EmailAddress = adminOtherEmail, IdentityId = identityId });
        }

        var addEmailResult = await _emailService.AddRangeAsync(adminOtherEmailList);
        if (!addEmailResult.IsSuccess)
        {
            NotifyErrorLocalized(addEmailResult.Message);
            return RedirectToAction(nameof(Index));
        }

        string link = Url.Action("index", "login", new { Area = "" }, Request.Scheme);

        var newUserMailDto = new NewUserMailDto { Email = addAdminResult.Data.Email, Url = link };

        // Use Hangfire to enqueue the email sending task
        BackgroundJob.Enqueue(() => _sendMailService.SendEmailNewAdmin(newUserMailDto));


        NotifySuccess($"{model.FirstName} {model.LastName}" + _stringLocalizer["the_person_was_successfully_added,_and_an_email_was_sent_to_their_email_address."]);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        var adminDeleteResponse = await _adminService.DeleteAsync(id);

        if (adminDeleteResponse.IsSuccess)
            NotifySuccessLocalized(adminDeleteResponse.Message);
        else
            NotifySuccessLocalized(adminDeleteResponse.Message);
        return Json(adminDeleteResponse);
    }
    [BreadcrumbName("Admin_Details")]
    public async Task<IActionResult> Details(Guid Id)
    {

        var getAdmin = await _adminService.GetDetailsByIdAsync(Id);
        if (getAdmin.IsSuccess)
        {
            var adminDetailsVM = _mapper.Map<AdminAdminDetailsVM>(getAdmin.Data);
            adminDetailsVM.OtherEmails = (await _emailService.GetEmailAddressesByIdentityIdAsync(getAdmin.Data.IdentityId)).Data;
            return View(adminDetailsVM);
        }

        NotifyError(getAdmin.Message);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Update(Guid id)
    {
        var getAdminResult = await _adminService.GetByIdAsync(id);
        if (!getAdminResult.IsSuccess)
        {
            NotifyErrorLocalized(getAdminResult.Message);
            return RedirectToAction(nameof(Index));
        }

        var adminDto = getAdminResult.Data;
        var adminUpdateVM = _mapper.Map<AdminAdminUpdateVM>(adminDto);
        adminUpdateVM.OtherEmails = (await _emailService.GetEmailAddressesByIdentityIdAsync(getAdminResult.Data.IdentityId)).Data;
        adminUpdateVM.Roles= (await _roleService.GetAllRoles()).Data;
        adminUpdateVM.SelectedRoleList =( await _roleService.GetUserRoles(id.ToString())).Data;
        return View(adminUpdateVM);
    }

    [HttpPost]
    public async Task<IActionResult> Update(AdminAdminUpdateVM model, IFormCollection collection)
    {

        if (!ModelState.IsValid)
        {
            return View(model);
        }
        var updateAdmin = _mapper.Map<AdminUpdateDto>(model);
        
        updateAdmin.FirstName = StringExtensions.TitleFormat(model.FirstName);
        updateAdmin.LastName = StringExtensions.TitleFormat(model.LastName);

        var updateAdminResult = await _adminService.UpdateAsync(updateAdmin);
        if (!updateAdminResult.IsSuccess)
        {
            NotifyErrorLocalized(updateAdminResult.Message);
            return View(model);
        }

        var otherEmailsList = collection["otherEmails"].ToList();
        var adminOtherEmailList = new List<EmailCreateDto>();
        var identityId = updateAdminResult.Data.IdentityId;

        foreach (var adminOtherEmail in otherEmailsList)
        {
            adminOtherEmailList.Add(new EmailCreateDto() { EmailAddress = adminOtherEmail, IdentityId = identityId });
        }
        var addEmailResult = await _emailService.UpdateRangeAsync(adminOtherEmailList, identityId);

        NotifySuccessLocalized(updateAdminResult.Message);
        return RedirectToAction(nameof(Index));
    }
    [HttpGet]
    public async Task<IActionResult> RoleUpdate(Guid id)
    {
        var user = await _adminService.GetByIdAsync(id);
        if (user == null)
        {
            NotifyError("User not found.");
            return RedirectToAction(nameof(Index));
        }

        // Kullanıcının rolünü Trainer olarak güncelle
        //    var roleUpdateResult = await _roleService.UpdateUserRole(new List<UserRoleAssingDto>
        //{
        //    new UserRoleAssingDto { Name = Roles.Trainer.ToString(), IsExist = true }
        //}, user.Data.IdentityId);


        // KULLANICININ ROLÜNÜ TRAINER OLARAK GÜNCELLER
        var roleUpdateResult = await _roleService.UserChangeRoleForAdminTrainer(user.Data.IdentityId, "Trainer");

        if (!roleUpdateResult.IsSuccess)
        {
            NotifyError(roleUpdateResult.Message);
            return RedirectToAction(nameof(Index));
        }

        // Kullanıcının IsActive durumunu false yap
        var admin = await _adminService.GetByIdentityIdAsync(user.Data.IdentityId);
        if (admin == null)
        {
            NotifyError(Messages.UserNotFound);
            return RedirectToAction(nameof(Index));
        }
        var adminTrainerResult = await _adminService.UpdateRoleAsync(user.Data.IdentityId, false);
        if (!adminTrainerResult.IsSuccess)
        {
            NotifyErrorLocalized(adminTrainerResult.Message);
            return RedirectToAction(nameof(Index));
        }


        // Önceden Trainer olarak atanmış mı?
        var trainer = await _trainerService.GetByIdentityIdAsync(user.Data.IdentityId);
        if (trainer.Data!=null) 
        {
            // VARSA STATUSUNU ACTIVE YAP
            var trainerUpdateResult = await _trainerService.UpdateRoleAsync(user.Data.IdentityId, true);
            if (!trainerUpdateResult.IsSuccess)
            {
                NotifyErrorLocalized(adminTrainerResult.Message);
                return RedirectToAction(nameof(Index));
            }
        }
        else
        {
            var technicalUnitId = (await _technicalUnitService.GetAllAsync()).Data.FirstOrDefault()?.Id;
            TrainerCreateDto dto = new()
            {
                Email = user.Data.Email,
                FirstName = user.Data.FirstName,
                LastName = user.Data.LastName,
                Gender = user.Data.Gender,
                TechnicalUnitId = technicalUnitId.Value,
                IdentityId = user.Data.IdentityId,
                OtherEmails = (await _emailService.GetEmailAddressesByIdentityIdAsync(user.Data.IdentityId)).Data
            };

            var addTrainerResult = await _trainerService.RoleAddAsync(dto);
            if (!addTrainerResult.IsSuccess)
            {
                NotifyErrorLocalized(addTrainerResult.Message);
                return RedirectToAction(nameof(Index));
            }
        }
        

        NotifySuccessLocalized("Admin_Successfully_Updated_To_Trainer.");
        return RedirectToAction(nameof(Index));
    }


    public async Task<AdminAdminUpdateVM> GetAdmin(Guid adminId)
    {
        var getAdminResult = await _adminService.GetByIdAsync(adminId);
        var adminDto = getAdminResult.Data;
        var adminUpdateVM = _mapper.Map<AdminAdminUpdateVM>(adminDto);
        adminUpdateVM.OtherEmails = (await _emailService.GetEmailAddressesByIdentityIdAsync(getAdminResult.Data.IdentityId)).Data;

        return adminUpdateVM;
    }

    public async Task<List<AdminAdminListVM>> GetAdminFromTable (string adminName)
    {
        var getAdmins = await _adminService.GetAllAsync();
        var admins = getAdmins.Data.Adapt<List<AdminAdminListVM>>();

        var adminsList = admins.Where(x=> x.FirstName.IndexOf(adminName, StringComparison.OrdinalIgnoreCase) >= 0 || x.LastName.IndexOf(adminName, StringComparison.OrdinalIgnoreCase) >=0 ).ToList()
                              .OrderBy(x=>x.FirstName).ToList();
        return adminsList;

    }

    [HttpGet]
    public async Task<IActionResult> SearchAdminFirstNameAutocomplete(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return Json(new List<object>());

        var admins = await _adminService.GetAllAsync();

        var matched = admins.Data
            .Where(x => x.FirstName.Contains(term, StringComparison.OrdinalIgnoreCase))
            .Select(x => x.FirstName)
            .Distinct()
            .Take(10)
            .ToList();

        return Json(matched);
    }

    [HttpGet]
    public async Task<IActionResult> SearchAdminLastNameAutocomplete(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return Json(new List<string>());

        var admins = await _adminService.GetAllAsync();

        var matched = admins.Data
            .Where(x => x.LastName.Contains(term, StringComparison.OrdinalIgnoreCase))
            .Select(x => x.LastName)
            .Distinct()
            .Take(10)
            .ToList();

        return Json(matched);
    }

    [HttpGet]
    public async Task<IActionResult> SearchAdminEmailAutocomplete(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return Json(new List<string>());

        var admins = await _adminService.GetAllAsync();

        var matched = admins.Data
            .Where(x => x.Email.Contains(term, StringComparison.OrdinalIgnoreCase))
            .Select(x => x.Email)
            .Distinct()
            .Take(10)
            .ToList();

        return Json(matched);
    }
}