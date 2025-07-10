using Elfie.Serialization;
using System.ComponentModel.DataAnnotations;

namespace BAExamApp.MVC.Areas.Admin.Models.TechnicalUnitVMs;

public class AdminTechnicalUnitListVM
{
    public Guid Id { get; set; }

    [Display(Name = "Technical_Unit_Name", ResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    public string Name { get; set; }
}