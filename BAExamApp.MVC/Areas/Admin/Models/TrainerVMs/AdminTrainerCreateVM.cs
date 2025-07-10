using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BAExamApp.MVC.Areas.Admin.Models.TrainerVMs
{
    public class AdminTrainerCreateVM
    {
        [Display(Name = "First_Name", ResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
        [Required(ErrorMessageResourceName = "Name_Can_Not_Be_Empty", ErrorMessageResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
        [MinLength(2, ErrorMessageResourceName = "Name_Must_Consist_Of_At_Least_2_Character", ErrorMessageResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
        public string FirstName { get; set; } = string.Empty;

        [Display(Name = "Last_Name", ResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
        [Required(ErrorMessageResourceName = "LastName_Can_Not_Be_Empty", ErrorMessageResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
        [MinLength(2, ErrorMessageResourceName = "LastName_Must_Consist_Of_At_Least_2_Character", ErrorMessageResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
        public string LastName { get; set; }

        [Display(Name = "Email", ResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
        [Required(ErrorMessageResourceName = "Email_Can_Not_Be_Empty", ErrorMessageResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
        [EmailAddress(ErrorMessageResourceName = "Email_Invalid", ErrorMessageResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
        public string Email { get; set; }

        [Display(Name = "Gender", ResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
        [Required(ErrorMessageResourceName = "Gender_Can_Not_Be_Empty", ErrorMessageResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
        public bool Gender { get; set; }

        [Display(Name = "Profile_Image")]
        public IFormFile? NewImage { get; set; }

        //[Display(Name = "TechnicalUnit_Name")]
        //[Required(ErrorMessage = "{0} alanı boş bırakılamaz.")]
        //public Guid? TechnicalUnitId { get; set; }
        public SelectList? TechnicalUnitList { get; set; }

        public List<Guid> ProductIds { get; set; }
        public SelectList? ProductList { get; set; }

        [Display(Name = "OtherEmails")]
        public List<string>? OtherEmails { get; set; }

        //[Display(Name = "Talent")]
       // public List<Guid>? TrainerTalentIds { get; set; }

       // [Display(Name = "Talent")]
       // public Guid? TalentId { get; set; } // Yetenek alanı null kabul edilebilir.
       // public SelectList? TalentList { get; set; }
    }
}
