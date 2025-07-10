namespace BAExamApp.MVC.Areas.Student.Models.StudentVMs;

public class StudentChangeProfilePictureVM
{
    [FromForm]
    public IFormFile File { get; set; }
}
