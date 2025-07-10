using BAExamApp.MVC.Areas.Admin.Models.NoteVMs;
using FluentValidation;

namespace BAExamApp.MVC.FluentValidators.NoteValidators;

public class NoteCreateValidator: AbstractValidator<AdminNoteCreateVM>
{
    public NoteCreateValidator()
    {
        RuleFor(x => x.Title).NotEmpty().WithMessage("Bu alan boş bırakılamaz");
        RuleFor(x => x.Content).NotEmpty().WithMessage("Bu alan boş bırakılamaz");
    }
}
