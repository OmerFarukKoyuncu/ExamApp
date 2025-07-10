using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.ComponentModel.DataAnnotations;


namespace BAExamApp.MVC.Areas.Admin.Models.AdminVMs;

public class AdminAdminCreateVM
{
    [Display(Name = "First_Name", ResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    [Required(ErrorMessageResourceName = "Name_Can_Not_Be_Empty", ErrorMessageResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    [MinLength(2, ErrorMessageResourceName = "Name_Must_Consist_Of_At_Least_2_Character", ErrorMessageResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    public string FirstName { get; set; } = string.Empty;

    [Display(Name = "Last_Name", ResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    [Required(ErrorMessageResourceName = "LastName_Can_Not_Be_Empty", ErrorMessageResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    [MinLength(2, ErrorMessageResourceName = "LastName_Must_Consist_Of_At_Least_2_Character", ErrorMessageResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    public string LastName { get; set; } = string.Empty;

    [Display(Name = "Email", ResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    [Required(ErrorMessageResourceName = "Email_Can_Not_Be_Empty", ErrorMessageResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    [EmailAddress(ErrorMessageResourceName = "Email_Invalid", ErrorMessageResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    public string Email { get; set; } = string.Empty;
    [Required(ErrorMessage = "Lütfen cinsiyet seçiniz.")]
    [Display(Name = "Gender")]
    public bool Gender { get; set; }
    //[Display(Name = "Date_Of_Birth")]
    //[Required(ErrorMessage = "{0} alanı boş bırakılamaz seçiniz.")]
    //[DataType(DataType.Date, ErrorMessage = "Lütfen geçerli bir tarih giriniz.")]
    //public DateTime DateOfBirth { get; set; }

    [Display(Name = "Profile_Image")]
    public IFormFile? NewImage { get; set; }

    //[Display(Name = "City")]
    //[Required(ErrorMessage = "{0} alanı boş bırakılamaz seçiniz.")]


    [Display(Name = "OtherEmails")]
    public string? OtherEmails { get; set; }
}
