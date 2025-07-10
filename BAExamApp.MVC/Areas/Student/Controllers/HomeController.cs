namespace BAExamApp.MVC.Areas.Student.Controllers;

public class HomeController : StudentBaseController
{
    private readonly IStudentService _studentService;
    public HomeController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    public async Task<IActionResult> Index()
    {
        var user = (await _studentService.GetByIdentityIdAsync(UserIdentityId)).Data;
        if (TempData["Login"] != null)
          HttpContext.Session.SetString("FullName", $"{user.FirstName} {user.LastName}");
        NotifySuccess($"Hoş Geldin {user?.FirstName} {user?.LastName}");
        return View();
    }
}