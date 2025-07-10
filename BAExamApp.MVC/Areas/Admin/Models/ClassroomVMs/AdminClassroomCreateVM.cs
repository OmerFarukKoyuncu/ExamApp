using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BAExamApp.MVC.Areas.Admin.Models.ClassroomVMs;

public class AdminClassroomCreateVM
{
    [Display(Name = "Classroom_Name", ResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    [Required(
    ErrorMessageResourceName = "Classroom_Name_Cannot_Be_Left_Empty",
    ErrorMessageResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    [MinLength(2,
    ErrorMessageResourceName = "Classroom_Name_Minimum_2_Character",
    ErrorMessageResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    public string Name { get; set; }

    [Display(Name = "Opening_Date", ResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    [Required(ErrorMessageResourceName = "Clasroom_Opening_Date_Cannot_Be_Empty",
              ErrorMessageResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    [DataType(DataType.Date)]
    public DateTime OpeningDate { get; set; } = DateTime.Now;

    [Display(Name = "Closed_Date", ResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    [Required(ErrorMessageResourceName = "Classroom_Closed_Date_Cannot_Be_Empty",
              ErrorMessageResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    [DataType(DataType.Date)]
    public DateTime ClosedDate { get; set; } = DateTime.Now;

    [Display(Name = "GroupType", ResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    [Required(ErrorMessageResourceName = "Classroom_Group_Type_Cannot_Be_Empty",
              ErrorMessageResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    public Guid GroupTypeId { get; set; }

    [Display(Name = "Branch", ResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    [Required(ErrorMessageResourceName = "Classroom_Branch_Cannot_Be_Empty",
              ErrorMessageResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    public Guid BranchId { get; set; }

    [Display(Name = "Product", ResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    [Required(ErrorMessageResourceName = "Classroom_Product_Cannot_Be_Empty",
              ErrorMessageResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    public List<Guid> ProductIds { get; set; }

    [Display(Name = "Trainer", ResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    [Required(ErrorMessageResourceName = "Classroom_Trainer_Cannot_Be_Empty",
             ErrorMessageResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    public List<Guid> TrainerIds { get; set; } = null!;
}