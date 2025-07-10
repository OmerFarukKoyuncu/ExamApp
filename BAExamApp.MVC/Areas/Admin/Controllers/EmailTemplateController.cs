using AutoMapper;
using BAExamApp.Dtos.Attributes;
using BAExamApp.Dtos.Emails;
using BAExamApp.Dtos.EmailTemplateDtos;
using BAExamApp.MVC.Areas.Admin.Models.EmailTemplateVMs;
using BAExamApp.MVC.Areas.Admin.Models.EmailVMs;
using Microsoft.AspNetCore.Mvc;

namespace BAExamApp.MVC.Areas.Admin.Controllers;
[BreadcrumbName("Email_Template")]
public class EmailTemplateController : AdminBaseController
{
    private readonly IEmailTemplateService _emailTemplateService;
    private readonly IMapper _mapper;

    public EmailTemplateController(IEmailTemplateService emailTemplateService, IMapper mapper)
    {
        _emailTemplateService = emailTemplateService;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _emailTemplateService.GetAllAsync();
        if (!result.IsSuccess)
        {
            return View(result);
        }
        var emailTemplateListVM = _mapper.Map<List<AdminEmailTemplateListVM>>(result.Data);
        return View(emailTemplateListVM);
    }
    [HttpPost]
    public async Task<IActionResult> Create(AdminEmailTemplateCreateVM adminEmailTemplateCreateVM)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction(nameof(Index));
        }
        var emailTemplateCreateDto = _mapper.Map<EmailTemplateCreateDto>(adminEmailTemplateCreateVM);
        var result = await _emailTemplateService.AddAsync(emailTemplateCreateDto);
        if (!result.IsSuccess)
        {
            return RedirectToAction(nameof(Index));
        }
        return RedirectToAction(nameof(Index));
    }
    [HttpGet]
    public async Task<IActionResult> Details(Guid Id)
    {
        var result = await _emailTemplateService.GetById(Id);
        if (!result.IsSuccess)
        {
            return RedirectToAction(nameof(Index));
        }
        var emailTemplateDetailsVM = _mapper.Map<AdminEmailTemplateDetailsVM>(result.Data);
        return View(emailTemplateDetailsVM);


    }

    [HttpGet]
    public async Task<IActionResult> Delete(Guid Id)
    {
        var result = await _emailTemplateService.DeleteAsync(Id);
        if (!result.IsSuccess)
        {
            return RedirectToAction(nameof(Index));
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Edit(AdminEmailTemplateUpdateVM adminEmailTemplateUpdateVM)
    {
        if (!ModelState.IsValid)
        {
            return RedirectToAction(nameof(Index));
        }
        var entityToUpdate = _mapper.Map<EmailTemplateUpdateDto>(adminEmailTemplateUpdateVM);
        var result = await _emailTemplateService.UpdateAsync(entityToUpdate);
        if (!result.IsSuccess)
        {
            return RedirectToAction(nameof(Index));
        }
        return RedirectToAction(nameof(Index));
    }
}
