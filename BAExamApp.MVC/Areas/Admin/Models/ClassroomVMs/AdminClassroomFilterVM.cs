using System.ComponentModel.DataAnnotations;

namespace BAExamApp.MVC.Areas.Admin.Models.ClassroomVMs;

public class AdminClassroomFilterVM
{
    public Guid Id { get; set; }

    [Display(Name = "Classroom_Name")]
    public string Name { get; set; }
}
