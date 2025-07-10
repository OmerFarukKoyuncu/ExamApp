using AutoMapper;
using BAExamApp.Business.Constants;
using BAExamApp.Core.Enums;
using BAExamApp.Core.Utilities.Results;
using BAExamApp.Dtos.Admins;
using BAExamApp.Dtos.Attributes;
using BAExamApp.Dtos.Emails;
using BAExamApp.Dtos.SendMails;
using BAExamApp.Dtos.Students;
using BAExamApp.Dtos.Trainers;
using BAExamApp.Dtos.Users;
using BAExamApp.MVC.Areas.Admin.Models.TrainerVMs;
using BAExamApp.MVC.Extensions;
using Hangfire;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Extensions.Localization;
using System;
using X.PagedList;
using TrainerAddedToClassromByAdminVM = BAExamApp.MVC.Areas.Admin.Models.TrainerVMs.TrainerAddedToClassromByAdminVM;


namespace BAExamApp.MVC.Areas.Admin.Controllers;
[BreadcrumbName("Trainers")]
public class TrainerController : AdminBaseController
{
    private readonly IBranchService _branchService;
    private readonly IClassroomService _classroomService;
    private readonly IEmailService _emailService;
    private readonly ITrainerService _trainerService;
    private readonly ITrainerClassroomService _trainerClassroomService;
    private readonly ITrainerProductService _trainerProductService;
    private readonly ITechnicalUnitService _technicalUnitService;
    private readonly ISendMailService _sendMailService;
    private readonly IProductService _productService;
    private readonly IMapper _mapper;
    private readonly IRoleService _roleService;
    private readonly IAdminService _adminService;
    private readonly IStringLocalizer<SharedModelResource> _stringLocalizer;
    public TrainerController(IBranchService branchService, IClassroomService classroomService, ITrainerService trainerService, ITechnicalUnitService technicalUnitService, ISendMailService sendMailService, IProductService productService, ITrainerProductService trainerProductService, IMapper mapper, IEmailService emailService, IRoleService roleService, IAdminService adminService, IStringLocalizer<SharedModelResource> stringLocalizer)
    {
        _branchService = branchService;
        _classroomService = classroomService;
        _emailService = emailService;
        _trainerService = trainerService;
        _technicalUnitService = technicalUnitService;
        _sendMailService = sendMailService;
        _productService = productService;
        _trainerProductService = trainerProductService;
        _mapper = mapper;
        _roleService = roleService;
        _adminService = adminService;
        _stringLocalizer = stringLocalizer;
    }


    [HttpGet]
    public async Task<IActionResult> Index(string firstName, string lastName, string email, int? page,int pageSize = 10, bool? showAllData = null, bool showCreateModal = false)
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
        var trainerList = _mapper.Map<IEnumerable<AdminTrainerListVM>>(result.Data)
            .Where(x => showAll || x.Status == Status.Active) 
            .Where(x => string.IsNullOrWhiteSpace(firstName) || x.FirstName.Contains(firstName, StringComparison.OrdinalIgnoreCase)) 
            .Where(x => string.IsNullOrWhiteSpace(lastName) || x.LastName.Contains(lastName, StringComparison.OrdinalIgnoreCase))   
            .Where(x => string.IsNullOrWhiteSpace(email) || x.Email.Contains(email, StringComparison.OrdinalIgnoreCase))          
            .Select(x => {
                x.FirstName = ToPascalCase(x.FirstName);
                x.LastName = ToPascalCase(x.LastName);
                return x;
            })
            .OrderBy(x => x.ModifiedDate)
            .ToList();
        
        foreach (var trainer in trainerList)
        {
            trainer.FirstName = ToPascalCase(trainer.FirstName);
            trainer.LastName = ToPascalCase(trainer.LastName);
        }

        trainerList = trainerList.OrderBy(trainer => trainer.ModifiedDate).ToList();
        ViewBag.ShowAllData = showAll;
        ViewBag.ShowCreateModal = showCreateModal;
        var paginatedList = trainerList.ToPagedList(pageNumber, pageSize);
        ViewBag.PageSize = pageSize;
        ViewBag.FirstName = firstName;
        ViewBag.LastName = lastName;
        ViewBag.Email = email;

        return View(paginatedList);
    }

    [HttpGet]
    public async Task<IActionResult> SearchTrainerFirstNameAutocomplete(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return Json(new List<string>());
        
        var trainers = await _trainerService.GetAllAsync();
        var matched = trainers.Data
            .Where(x => x.FirstName.Contains(term, StringComparison.OrdinalIgnoreCase))
            .Select(x => x.FirstName)
            .Distinct()
            .Take(10)
            .ToList();

        return Json(matched);
    }
    
    [HttpGet]
    public async Task<IActionResult> SearchTrainerLastNameAutocomplete(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return Json(new List<string>());
        
        var trainers = await _trainerService.GetAllAsync();
        var matched = trainers.Data
            .Where(x => x.LastName.Contains(term, StringComparison.OrdinalIgnoreCase))
            .Select(x => x.LastName)
            .Distinct()
            .Take(10)
            .ToList();

        return Json(matched);
    }
    
    [HttpGet]
    public async Task<IActionResult> SearchTrainerEmailAutocomplete(string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return Json(new List<string>());
        
        var trainers = await _trainerService.GetAllAsync();
        var matched = trainers.Data
            .Where(x => x.Email.Contains(term, StringComparison.OrdinalIgnoreCase))
            .Select(x => x.Email)
            .Distinct()
            .Take(10)
            .ToList();

        return Json(matched);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(AdminTrainerCreateVM model, IFormCollection collection)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction(nameof(Index), new { showCreateModal = true });
        }

        var trainerCreateDto = _mapper.Map<TrainerCreateDto>(model);

        trainerCreateDto.FirstName = StringExtensions.TitleFormat(model.FirstName);
        trainerCreateDto.LastName = StringExtensions.TitleFormat(model.LastName);

        var addTrainerResult = await _trainerService.AddAsync(trainerCreateDto);
        if (!addTrainerResult.IsSuccess)
        {
            NotifyErrorLocalized(addTrainerResult.Message);
            return RedirectToAction(nameof(Index));
        }

        string link = Url.Action("index", "login", new { Area = "" }, Request.Scheme);
        BackgroundJob.Enqueue(() => _sendMailService.SendEmailNewTrainer(new NewUserMailDto { Email = addTrainerResult.Data.Email, Url = link }));

        NotifySuccess($"{model.FirstName} {model.LastName} kişisi başarıyla eklendi, Mail adresine mail gönderildi.");

        var trainerOtherEmailList = new List<EmailCreateDto>();
        var otherEmailsList = collection["otherEmails"].ToList();
        var identityId = addTrainerResult.Data.IdentityId;
        foreach (var trainerOtherEmail in otherEmailsList)
        {
            trainerOtherEmailList.Add(new EmailCreateDto() { EmailAddress = trainerOtherEmail, IdentityId = identityId });
        }
        var addEmailResult = await _emailService.AddRangeAsync(trainerOtherEmailList);

        if (!addEmailResult.IsSuccess)
        {
            NotifyErrorLocalized(addEmailResult.Message);
            return RedirectToAction(nameof(Index));
        }

        return RedirectToAction(nameof(Index));
    }
    [HttpGet]
    public async Task<IActionResult> Delete(Guid id)
    {
        var trainerDeleteResult = await _trainerService.DeleteAsync(id);

        if (!trainerDeleteResult.IsSuccess)
        {
            NotifyErrorLocalized(trainerDeleteResult.Message);
        }
        else NotifySuccessLocalized(trainerDeleteResult.Message);

        return Json(trainerDeleteResult);
    }

    [HttpGet]
    public async Task<IActionResult> Update(Guid id)
    {
        var trainerFoundResult = await _trainerService.GetByIdAsync(id);
        if (!trainerFoundResult.IsSuccess)
        {
            NotifyErrorLocalized(trainerFoundResult.Message);
            return RedirectToAction(nameof(Index));
        }

        var trainerUpdateVM = _mapper.Map<AdminTrainerUpdateVM>(trainerFoundResult.Data);
        trainerUpdateVM.TechnicalUnitList = await GetTechnicalUnitsAsync(trainerUpdateVM.TechnicalUnitId);
        trainerUpdateVM.OtherEmails = (await _emailService.GetEmailAddressesByIdentityIdAsync(trainerFoundResult.Data.IdentityId)).Data;

        return PartialView("Update", trainerUpdateVM);
    }


    [HttpPost]
    public async Task<IActionResult> Update(AdminTrainerUpdateVM trainerUpdateVM, IFormCollection collection)
    {
        if (!ModelState.IsValid)
        {
            trainerUpdateVM.TechnicalUnitList = await GetTechnicalUnitsAsync(trainerUpdateVM.TechnicalUnitId);
            return View(trainerUpdateVM);
        }

        var trainerUpdateDto = _mapper.Map<TrainerUpdateDto>(trainerUpdateVM);
        //if (trainerUpdateVM.NewImage != null)
        //{
        //    trainerUpdateDto.Image = await trainerUpdateVM.NewImage.FileToStringAsync();
        //}

        trainerUpdateDto.FirstName = StringExtensions.TitleFormat(trainerUpdateVM.FirstName);
        trainerUpdateDto.LastName = StringExtensions.TitleFormat(trainerUpdateVM.LastName);

        var updateTrainerresult = await _trainerService.UpdateAsync(trainerUpdateDto);
        if (!updateTrainerresult.IsSuccess)
        {
            NotifyErrorLocalized(updateTrainerresult.Message);
        }
        else
        {
            NotifySuccessLocalized(updateTrainerresult.Message);
        }

        var otherEmailsList = collection["otherEmails"].ToList();
        var trainerOtherEmailList = new List<EmailCreateDto>();
        var identityId = updateTrainerresult.Data.IdentityId;

        foreach (var trainerOtherEmail in otherEmailsList)
        {
            trainerOtherEmailList.Add(new EmailCreateDto() { EmailAddress = trainerOtherEmail, IdentityId = identityId });
        }
        var addEmailResult = await _emailService.UpdateRangeAsync(trainerOtherEmailList, identityId);

        return RedirectToAction(nameof(Index));
    }

    /// <summary>
    /// Role Güncelleme İşlemi İçin Kulalnılır. Diğer güncelleme İşlemleri için KULLANILMAZ!!!!!!
    /// </summary>
    /// <param name="id">Guid  TrainerID</param>
    [HttpGet]
    public async Task<IActionResult> RoleUpdate(Guid id)
    {
        var user = await _trainerService.GetByIdAsync(id);
        if (user == null)
        {
            NotifyError("User not found.");
            return RedirectToAction(nameof(Index));
        }

        // KULLANICININ ROLÜNÜ TRAINER OLARAK GÜNCELLER
        var roleUpdateResult = await _roleService.UserChangeRoleForAdminTrainer(user.Data.IdentityId, "Admin");

        if (!roleUpdateResult.IsSuccess)
        {
            NotifyError(roleUpdateResult.Message);
            return RedirectToAction(nameof(Index));
        }

        // Kullanıcının IsActive durumunu false yap
        var trainer = await _trainerService.GetByIdentityIdAsync(user.Data.IdentityId);
        if(trainer == null)
        {
            NotifyError(Messages.UserNotFound);
            return RedirectToAction(nameof(Index));
        }
        var trainerAdminResult = await _trainerService.UpdateRoleAsync(user.Data.IdentityId, false);
        if (!trainerAdminResult.IsSuccess)
        {
            NotifyErrorLocalized(trainerAdminResult.Message);
            return RedirectToAction(nameof(Index));
        }

        var adminResult = await _adminService.GetByIdentityIdAsync(user.Data.IdentityId);
        AdminDto adminListDtos = adminResult.Data;

        // Eğer adminListDtos null ise yeni bir AdminCreateDto oluştur ve ekle
        if (adminResult.Data == null)
        {
            AdminCreateDto dto = new()
            {
                Email = user.Data.Email,
                FirstName = user.Data.FirstName,
                LastName = user.Data.LastName,
                Gender = user.Data.Gender,
                IdentityId = user.Data.IdentityId,
            };

            var addTrainerResult = await _adminService.RoleAddAsync(dto);
            if (!addTrainerResult.IsSuccess)
            {
                NotifyErrorLocalized(addTrainerResult.Message);
                return RedirectToAction(nameof(Index));
            }
        }
        else // Eğer adminListDtos null değilse, var olanı aktif et
        {
            var admin = await _adminService.GetByIdentityIdAsync(adminResult.Data.IdentityId);
            if(admin == null)
            {
                NotifyError(Messages.UserNotFound);
                return RedirectToAction(nameof(Index));
            }
            var adminUpdateResult = await _adminService.UpdateRoleAsync(admin.Data.IdentityId, true);
            if (!adminUpdateResult.IsSuccess)
            {
                NotifyError(adminUpdateResult.Message);
                return RedirectToAction(nameof(Index));
            }
        }

        NotifySuccessLocalized("Trainer_Successfully_Updated_To_Admin.");
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id)
    {
        ViewBag.TecnicalUnit = await GetTechnicalUnitsAsync();

        var getTrainerResponse = await _trainerService.GetTrainerDetailsByIdAsync(id);

        if (getTrainerResponse.IsSuccess)
        {
            var trainerDetails = _mapper.Map<AdminTrainerDetailsVM>(getTrainerResponse.Data);
            trainerDetails.OtherEmails = (await _emailService.GetEmailAddressesByIdentityIdAsync(getTrainerResponse.Data.IdentityId)).Data;

            // İsim ve soyadı formatlama
            trainerDetails.FirstName = trainerDetails.FirstName.ToPascalCaseWithSpaces();
            trainerDetails.LastName = trainerDetails.LastName.CapitalizeFirstLetter();

            return View(trainerDetails);
        }

        NotifyErrorLocalized(getTrainerResponse.Message);
        return RedirectToAction(nameof(Index));
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


    [HttpGet]
    public IActionResult CallProductListTrainerUpdate(Guid technicalUnitId)
    {
        return ViewComponent("TrainerUpdateProductList", new { technicalUnitId });
    }

    /// <summary>
    /// Verilen ClassRoomId ye Göre var olan Trainerların First Name ve Last Name Alanlarını Getirir.
    /// </summary>
    /// <param name="classroomId">Guid  classroomId</param>
    /// <returns>Listeyi string olarak döner</returns>
    private async Task<string> GetClassroomTrainersAsync(Guid classroomId)
    {
        var result = await _trainerClassroomService.GetTrainersWithSpesificClassroomIdAsync(classroomId);
        if (!result.IsSuccess)
        {
            return result.Message;
        }

        var trainerList = result.Data.Select(x => x.FirstName + " " + x.LastName).ToList();
        return string.Join($" | ", trainerList);
    }
    /// <summary>
    /// Eğitimleri liste olarak getirir
    /// </summary>
    /// <param name="technicalUnitId"> teknik birim id ye göre eğitimleri getirir</param>
    /// <returns>Parametre ile kullanılırsa parametre verisine göre json dönüş yapar</returns>
    [HttpGet]
    public async Task<IActionResult> CallProductList(Guid technicalUnitId)
    {
        var productList = await _productService.GetAllByTechnicalUnitIdAsync(technicalUnitId);
        var selectList = new List<SelectListItem>();
        foreach (var product in productList.Data)
        {
            selectList.Add(new SelectListItem
            {
                Value = product.Id.ToString(),
                Text = product.Name
            });
        }
        return Json(selectList);
    }
   
   

    public async Task<AdminTrainerUpdateVM> GetTrainer(Guid trainerId)
    {
        var trainerFoundResult = await _trainerService.GetByIdAsync(trainerId);

        var trainerUpdateVM = _mapper.Map<AdminTrainerUpdateVM>(trainerFoundResult.Data);
        trainerUpdateVM.TechnicalUnitList = await GetTechnicalUnitsAsync(trainerUpdateVM.TechnicalUnitId);
        trainerUpdateVM.OtherEmails = (await _emailService.GetEmailAddressesByIdentityIdAsync(trainerFoundResult.Data.IdentityId)).Data;

        return trainerUpdateVM;
    }

    /// <summary>
    /// Converts a given string to PascalCase format.
    /// </summary>
    /// <param name="input">The input string to be converted.</param>
    /// <returns>A string converted to PascalCase format.</returns>
    private string ToPascalCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;
        return string.Join(" ", input.Split(' ')
                                     .Where(w => !string.IsNullOrEmpty(w))
                                     .Select(w => char.ToUpper(w[0]) + w.Substring(1).ToLower()));
    }
    [HttpGet]
    public async Task<IActionResult> UpdateStatus(Guid id)
    {
        var user = await _trainerService.GetByIdAsync(id);
        if (user == null)
        {
            NotifyError("User not found.");
            return RedirectToAction(nameof(Index));
        }

        // Kullanıcının mevcut durumunu kontrol edin ve tersine çevirin
        TrainerDto trainerDto = user.Data.Adapt<TrainerDto>();
        TrainerUpdateDto updateDto = trainerDto.Adapt<TrainerUpdateDto>();
        updateDto.Status = trainerDto.Status == Status.Active ? Status.Passive : Status.Active;

        var trainerAdminResult = await _trainerService.UpdateAsync(updateDto);
        if (!trainerAdminResult.IsSuccess)
        {
            NotifyErrorLocalized(trainerAdminResult.Message);
            return RedirectToAction(nameof(Index));
        }
        NotifySuccessLocalized("Trainer status successfully updated.");
        return RedirectToAction(nameof(Index));
    }
    public async Task<List<AdminTrainerListVM>> GetTrainerFromTable(string trainerName)
    {
        var getTrainers = await _trainerService.GetAllAsync();
        var trainers = getTrainers.Data.Adapt<List<AdminTrainerListVM>>();

        // Hem ad soyad birleştirilmiş olarak ara hem de ayrı ayrı ara
        var trainerList = trainers
            .Where(x =>
                $"{x.FirstName} {x.LastName}".Contains(trainerName, StringComparison.OrdinalIgnoreCase) ||
                x.FirstName.Contains(trainerName, StringComparison.OrdinalIgnoreCase) ||
                x.LastName.Contains(trainerName, StringComparison.OrdinalIgnoreCase)
            )
            .OrderBy(x => x.FirstName)
            .ToList();

        return trainerList;
    }

    [HttpGet]
    public async Task<IActionResult> AddClassroom(Guid id)
    {
        Models.TrainerVMs.TrainerAddedToClassromByAdminVM viewModel = new()
        {
            TrainerId = id,
            Classrooms = await GetClassroomsAsync(id)
        };
        //try
        //{
        //    viewModel.AppointedTrainersId = (await _trainerClassroomService.GetTrainersWithSpesificClassroomIdAsync(id))
        //        .Data
        //        .Select(x => x.Id.ToString())
        //        .ToList();
        //}
        //catch (Exception)
        //{
        //    viewModel.AppointedTrainersId = new List<string>();
        //}
        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> AddClassroom(TrainerAddedToClassromByAdminVM viewModel)
    {
        if (!ModelState.IsValid)
        {
            viewModel.Classrooms = await GetTrainersAsync(viewModel.TrainerId);
            return View(viewModel);
        }

        var addTrainerResponse = await _adminService.AddClassRoomsToTrainers(_mapper.Map<TrainerAddedToClassroomByAdminDto>(viewModel));
        if (addTrainerResponse.IsSuccess)
        {
            NotifySuccessLocalized(addTrainerResponse.Message);
        }
        else
        {
            NotifyErrorLocalized(addTrainerResponse.Message);
        }

        return RedirectToAction(nameof(Index));
    }



    
    

    private async Task<List<SelectListItem>> GetTrainersAsync(Guid classroomId)
    {
        var getFreeTrainersResponse = await _trainerService.GetAllActiveAsync();
        if (getFreeTrainersResponse.IsSuccess)
        {
            var trainerList = getFreeTrainersResponse.Data.Select(x => new SelectListItem()
            {
                Value = x.Id.ToString(),
                Text = x.FirstName + " " + x.LastName,
            }).ToList();

            return trainerList;
        }
        return new List<SelectListItem>();
    }
    private async Task<List<SelectListItem>> GetClassroomsAsync(Guid trainerId)
    {
        var alLClassrooms = await _classroomService.GetAllAsync();
        if (alLClassrooms.IsSuccess)
        {
            var classList = alLClassrooms.Data.Select(x => new SelectListItem()
            {
                Value = x.Id.ToString(),
                Text = x.Name,
            }).ToList();

            return classList;
        }
        return new List<SelectListItem>();
    }  

}
