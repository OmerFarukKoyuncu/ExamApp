using System.ComponentModel.DataAnnotations;

namespace BAExamApp.MVC.Areas.Admin.Models.BranchVMs;

public class AdminBranchUpdateVM
{
    public Guid Id { get; set; }

    [Display(Name = "Branch_Name", ResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    [Required(ErrorMessageResourceName="Branch_Name_Cannot_Be_Left_Empty", ErrorMessageResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    [MinLength(2, ErrorMessageResourceName="Branch_Name_Minimum_2_Character", ErrorMessageResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    [MaxLength(256, ErrorMessageResourceName="Branch_Name_Maximum_256_Character", ErrorMessageResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    public string Name { get; set; }
}