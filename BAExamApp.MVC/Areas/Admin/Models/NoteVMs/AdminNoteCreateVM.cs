using System.ComponentModel.DataAnnotations;

namespace BAExamApp.MVC.Areas.Admin.Models.NoteVMs;

public class AdminNoteCreateVM
{
    [Required(ErrorMessage = "Bu alan boş bırakılamaz.")]
    public string Title { get; set; }

    [Required(ErrorMessage = "Bu alan boş bırakılamaz.")]
    public string Content { get; set; }

    [Required(ErrorMessage = "Bu alan boş bırakılamaz.")]
    [DataType(DataType.Date)]
    public DateTime Date { get; set; }
}
