using BAExamApp.Entities.DbSets;
using BAExamApp.MVC.Areas.Admin.Models.ClassroomVMs;
using System.ComponentModel.DataAnnotations;

namespace BAExamApp.MVC.Areas.Admin.Models.GroupTypeVMs;

public class AdminGroupTypeDetailVM
{
    public Guid Id { get; set; }

    [Display(Name = "Group_Type")]
    public string Name { get; set; }

    [Display(Name = "Information")]
    public string Information { get; set; }

    [Display(Name = "Classrooms")]
    public List<AdminClassroomDetailsVM> Classrooms { get; set; } = new List<AdminClassroomDetailsVM>();
}