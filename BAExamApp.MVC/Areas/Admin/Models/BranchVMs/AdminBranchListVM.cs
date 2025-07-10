using System.ComponentModel.DataAnnotations;

namespace BAExamApp.MVC.Areas.Admin.Models.BranchVMs;

public class AdminBranchListVM
{
    public Guid Id { get; set; }

    [Display(Name = "Branch_Name", ResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    public string Name { get; set; }
}