using System.ComponentModel.DataAnnotations;

namespace BAExamApp.MVC.Areas.Admin.Models.GroupTypeVMs;

public class AdminGroupTypeCreateVM
{
    [Display(Name = "Group_Type", ResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    [Required(ErrorMessageResourceName = "Group_Type_Cannot_Be_Left_Empty",
               ErrorMessageResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    [MinLength(2, ErrorMessageResourceName = "Group_Type_Minimum_2_Character",
                ErrorMessageResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    [RegularExpression(@"^(?!'+$)[a-zA-Z'ğüşöçİĞÜŞÖÇ]+(?:\s+[a-zA-Z'ğüşöçİĞÜŞÖÇ]+)*$",
         ErrorMessageResourceName = "Group_Type_Cannot_Include_Special_Character_Or_Digit",
         ErrorMessageResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    public string Name { get; set; }

    [Display(Name = "Information", ResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    [Required(ErrorMessageResourceName = "Information_Cannot_Be_Left_Empty",
              ErrorMessageResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    [MaxLength(500, ErrorMessageResourceName = "Information_Maximum_500_Character",
               ErrorMessageResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    public string Information { get; set; }
}

