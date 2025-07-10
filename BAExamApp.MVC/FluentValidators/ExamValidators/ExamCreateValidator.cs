using BAExamApp.MVC.Areas.Trainer.Models.ExamVMs;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace BAExamApp.MVC.FluentValidators.ExamValidators;

public class ExamCreateValidator : AbstractValidator<TrainerExamCreateVM>
{
    public ExamCreateValidator(IStringLocalizer<TrainerExamCreateVM> stringLocalizer)
    {
        //RuleFor(x => x.ExamDateTime).NotEmpty().WithMessage("Bu alan boş bırakılamaz")
        //  .GreaterThan(DateTime.Today.AddHours(-1)).WithMessage("Geçmiş bir tarih seçemezsiniz");

        RuleFor(x => x.ExamDuration).NotEmpty().WithMessage(stringLocalizer["Please_Do_Not_Leave_It_Blank"]);

        RuleFor(x => x.ExamType).NotEmpty().WithMessage(stringLocalizer["Please_Do_Not_Leave_It_Blank"]);

        RuleFor(x => x.ExamCreationType).NotEmpty().WithMessage(stringLocalizer["Please_Do_Not_Leave_It_Blank"]);

        RuleFor(x => x.MaxScore).GreaterThan(0).WithMessage(stringLocalizer["Value_Greater_Than"]);

        RuleFor(x => x.BonusScore).GreaterThanOrEqualTo(0).WithMessage(stringLocalizer["Please_Enter_Natural_Number"]);

        RuleFor(x => x.ExamRuleId).NotEmpty().WithMessage(stringLocalizer["Please_Enter_Exam_Rule"]);

        RuleFor(x => x.ExamClassroomsIds).NotEmpty().WithMessage(stringLocalizer["Please_Select_Class"]);

        RuleFor(x => x.ExamDateTime).GreaterThanOrEqualTo(DateTime.Now).WithMessage(stringLocalizer["Exam_DateTime"]);

        RuleFor(vm => vm.Description)
            .Cascade(CascadeMode.StopOnFirstFailure)
            .NotEmpty().When(vm => !vm.forClassroom)
            .WithMessage(stringLocalizer["Student_Description"]);
    }
}