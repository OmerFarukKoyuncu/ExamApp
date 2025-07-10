using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BAExamApp.MVC.Areas.Admin.Models.SubjectVMs;

public class AdminSubjectCreateVM
{
    [DisplayName("Subject")]
    [Required(ErrorMessage = "Required_Field_Error")]
    [MinLength(2, ErrorMessage = "Konu adı en az 2 karakterden oluşmalıdır.")]
    public string Name { get; set; }

    [Display(Name = "Product_Name")]
    [Required(ErrorMessage = "Required_Field_Error")]
    public List<Guid> ProductIds { get; set; }

    public SelectList? ProductSelectList { get; set; }
    
}