using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BAExamApp.MVC.Areas.Admin.Models.ApiUserVMs
{
    public class AdminApiUserCreateVM
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


        [Display(Name = "Profile_Image")]
        public IFormFile? NewImage { get; set; }


    }
}