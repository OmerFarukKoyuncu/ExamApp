using BAExamApp.Dtos.StudentClassrooms;
using BAExamApp.Dtos.TrainerClassrooms;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BAExamApp.MVC.Areas.Admin.Models.ClassroomVMs;

public class AdminClassroomUpdateVM
{

    public Guid Id{ get; set; }
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
    public DateTime OpeningDate { get; set; }

    [Display(Name = "Closed_Date", ResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    [Required(ErrorMessageResourceName = "Classroom_Closed_Date_Cannot_Be_Empty",
              ErrorMessageResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    [DataType(DataType.Date)]
    public DateTime ClosedDate { get; set; }

    [Display(Name = "GroupType", ResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    [Required(ErrorMessageResourceName = "Classroom_Group_Type_Cannot_Be_Empty",
              ErrorMessageResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    public Guid GroupTypeId { get; set; }

    [Display(Name = "Branch", ResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    [Required(ErrorMessageResourceName = "Classroom_Branch_Cannot_Be_Empty",
              ErrorMessageResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    public Guid BranchId { get; set; }

    public SelectList? GroupTypeList { get; set; }
    public SelectList? BranchList { get; set; }

    [Display(Name = "Product_List")]
    public SelectList? ProductList { get; set; }

    [Display(Name = "Product", ResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    [Required(ErrorMessageResourceName = "Classroom_Product_Cannot_Be_Empty",
               ErrorMessageResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    public List<Guid>? ProductIds { get; set; }
    public List<StudentClassroomListForClassroomDetailsForAdminDto> StudentClassrooms { get; set; }
    public List<TrainerClassroomListForClassroomDetailsDto> TrainerClassrooms { get; set; }

    
}