
using System.ComponentModel.DataAnnotations;

namespace BAExamApp.MVC.Areas.Student.Models.ClassroomVMs;

public class StudentClassroomFilterVM
{
    public Guid Id { get; set; }

    [Display(Name = "Classroom_Name")]
    public string Name { get; set; }   
}