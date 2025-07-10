using System.ComponentModel.DataAnnotations;

namespace BAExamApp.MVC.Areas.Admin.Models.TechnicalUnitVMs;

public class AdminTechnicalUnitUpdateVM
{
    public Guid Id { get; set; }
    [Display(Name = "Technical_Unit_Name", ResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    [Required(ErrorMessageResourceName = "Technical_Unit_Name_Required", ErrorMessageResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    [MinLength(2, ErrorMessageResourceName = "Technical_Unit_Name_MinLength", ErrorMessageResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    [MaxLength(500, ErrorMessageResourceName = "Technical_Unit_Name_MaxLength", ErrorMessageResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    public string Name { get; set; }
}