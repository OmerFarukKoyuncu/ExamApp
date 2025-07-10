using AutoMapper;
using BAExamApp.Dtos.Students;
using BAExamApp.MVC.Areas.Student.Models.ClassroomVMs;
using BAExamApp.MVC.Areas.Student.Models.StudentClassroomVMs;
using BAExamApp.MVC.Areas.Student.Models.StudentVMs;
using BAExamApp.MVC.Extensions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using X.PagedList;
using BAExamApp.MVC.Resources;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace BAExamApp.MVC.Areas.Student.Controllers;

public class StudentController : StudentBaseController
{
    private readonly IStudentService _studentService;
    private readonly IMapper _mapper;
    private readonly IClassroomService _classroomService;
    private readonly IStringLocalizer<SharedModelResource> _localizer;

    public StudentController(
        IStudentService studentService,
        IMapper mapper,
        IClassroomService classroomService,
        IStringLocalizer<SharedModelResource> localizer)
    {
        _studentService = studentService;
        _mapper = mapper;
        _classroomService = classroomService;
        _localizer = localizer;
    }

    [HttpGet]
    [HttpGet]
    public async Task<IActionResult> Details()
    {
        var getStudent = await _studentService.GetByIdentityIdAsync(UserIdentityId);
        if (getStudent.IsSuccess)
        {
            // Rol çevirisini burada ViewBag’e veriyoruz
            ViewBag.Role = User.IsInRole("Student") ? _localizer["Student"] : "";

            return View(_mapper.Map<StudentStudentDetailVM>(getStudent.Data));
        }
        NotifyErrorLocalized(getStudent.Message);
        return RedirectToAction("Index", "Home");
    }



    [HttpGet]
    public async Task<IActionResult> ClassroomList()
    {
        var getStudent = await _studentService.GetByIdentityIdAsync(UserIdentityId);
        var classrommsDto = await _classroomService.GetActivelassroomsByStudentIdAsync(getStudent.Data.Id);

        var classroomListVm = classrommsDto.Data.Adapt<List<StudentClassroomFilterVM>>();
        ViewBag.ClassList = classroomListVm.Select(c => new SelectListItem { Text = c.Name, Value = c.Id.ToString() }).ToList();

        var paginatedList = new List<StudentStudentListVM>().ToPagedList(1, 10);

        return View(paginatedList);
    }

    [HttpGet]
    public async Task<IActionResult> GetFilteredList(string? classroomName, int page = 1, int pageSize = 10)
    {
        List<StudentStudentListVM>? studentList = new List<StudentStudentListVM>();

        if (!string.IsNullOrEmpty(classroomName))
        {
            var getStudentResponse = await _studentService.GetActiveStudentsByClassroomIdAsync(Guid.Parse(classroomName));
            studentList = _mapper.Map<List<StudentStudentListVM>>(getStudentResponse.Data);
        }
        else
        {
            var getStudent = await _studentService.GetByIdentityIdAsync(UserIdentityId);
            var classrommsDto = await _classroomService.GetActivelassroomsByStudentIdAsync(getStudent.Data.Id);

            foreach (var classroom in classrommsDto.Data)
            {
                var getStudentResponse = await _studentService.GetActiveStudentsByClassroomIdAsync(classroom.Id);
                studentList?.AddRange(_mapper.Map<List<StudentStudentListVM>>(getStudentResponse.Data));
            }
        }

        var paginatedList = studentList.ToPagedList(page, pageSize);

        ViewBag.ClassList = await GetClasses(classroomName);
        ViewBag.PageSize = pageSize;
        ViewBag.ClassroomName = classroomName;

        return View("ClassroomList", paginatedList);
    }

    [HttpPost]
    public async Task<IActionResult> ChangeProfilePicture(IFormFile profilePicture)
    {
        var file = await profilePicture.FileToStringAsync();
        var getStudent = await _studentService.GetByIdentityIdAsync(UserIdentityId);


        await _studentService.ChangeProfilePicture(getStudent.Data.Id, file);
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public async Task<IActionResult> RemoveProfilePicture()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var student = await _studentService.GetByIdentityIdAsync(userId);

        if (student.IsSuccess)
        {
            await _studentService.RemoveProfilePictureAsync(student.Data.Id);
        }

        return RedirectToAction("Index", "Home");
    }

    private async Task<SelectList> GetClasses(string? defaultValue = null)
    {
        var getStudent = await _studentService.GetByIdentityIdAsync(UserIdentityId);
        var classrommsList = await _classroomService.GetActivelassroomsByStudentIdAsync(getStudent.Data.Id);
        return new SelectList(classrommsList.Data.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = x.Name
        }), "Value", "Text", selectedValue: defaultValue);
    }


}
