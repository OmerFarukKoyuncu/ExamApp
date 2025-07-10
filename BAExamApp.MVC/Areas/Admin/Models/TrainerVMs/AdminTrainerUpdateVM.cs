using BAExamApp.Core.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BAExamApp.MVC.Areas.Admin.Models.TrainerVMs;

public class AdminTrainerUpdateVM
{
    public Guid Id { get; set; }

    [Display(Name = "First_Name", ResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    [Required(ErrorMessageResourceName = "Name_Can_Not_Be_Empty", ErrorMessageResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    [MinLength(2, ErrorMessageResourceName = "Name_Must_Consist_Of_At_Least_2_Character", ErrorMessageResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    public string FirstName { get; set; } = string.Empty;

    [Display(Name = "Last_Name", ResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    [Required(ErrorMessageResourceName = "LastName_Can_Not_Be_Empty", ErrorMessageResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    [MinLength(2, ErrorMessageResourceName = "LastName_Must_Consist_Of_At_Least_2_Character", ErrorMessageResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    public string LastName { get; set; } = string.Empty;

    [Display(Name = "Gender", ResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    [Required(ErrorMessageResourceName = "Gender_Can_Not_Be_Empty", ErrorMessageResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    public bool? Gender { get; set; }

    //[Display(Name = "Date_Of_Birth")]
    //[Required(ErrorMessage = "{0} alanı boş bırakılamaz.")]
    //public DateTime DateOfBirth { get; set; }

    [Display(Name = "Profile_Image")]
    public IFormFile? NewImage { get; set; }
    public byte[]? OriginalImage { get; set; }
    public string? Image { get; set; }
    public bool RemoveImage { get; set; }

    [Display(Name = "TechnicalUnit_Name")]
    [Required(ErrorMessage = "{0} alanı boş bırakılamaz.")]
    public Guid TechnicalUnitId { get; set; }
    public SelectList? TechnicalUnitList { get; set; }

    [Display(Name = "Product")]
    public List<Guid>? ProductIds { get; set; }

    [Display(Name = "Product_List")]
    public SelectList? ProductList { get; set; }

    [Display(Name = "Email", ResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    [Required(ErrorMessageResourceName = "Email_Can_Not_Be_Empty", ErrorMessageResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    [EmailAddress(ErrorMessageResourceName = "Email_Invalid", ErrorMessageResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "OtherEmails")]
    public List<string>? OtherEmails { get; set; }

    public Status Status { get; set; }

    //[Display(Name = "Talent_Names")]
    //[Required(ErrorMessage = "{0} alanı boş bırakılamaz.")]
    //public List<Guid>? TrainerTalentIds { get; set; }

   // [Display(Name = "Talent_Names")]
    //public SelectList? TalentList { get; set; }
}