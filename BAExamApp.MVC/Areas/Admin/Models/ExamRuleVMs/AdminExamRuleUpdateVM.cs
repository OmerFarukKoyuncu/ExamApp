using BAExamApp.Entities.Enums;
using BAExamApp.MVC.Areas.Admin.Models.ExamRuleSubtopicVMs;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BAExamApp.MVC.Areas.Admin.Models.ExamRuleVMs;

public class AdminExamRuleUpdateVM
{
    public Guid Id { get; set; }

    [Display(Name = "Exam_Rule_Name")]
    [Required(ErrorMessage = "Required_Field_Error")]
    public string Name { get; set; }

    [Display(Name = "Exam_Rule_Description")]
    [Required(ErrorMessage = "Required_Field_Error")]
    public string? Description { get; set; }

    [Display(Name = "Product")]
    [Required(ErrorMessage = "Required_Field_Error")]
    public Guid ProductId { get; set; }

    [Display(Name = "Exam_Type")]
    [Required(ErrorMessage = "Required_Field_Error")]
    public ExamType ExamType { get; set; }

    [Display(Name = "Question_Type")]
    public QuestionType QuestionType { get; set; }

    [Display(Name = "Question_Difficulty")]
    public Guid QuestionDifficultyId { get; set; }

    [Display(Name = "Subject")]
    public Guid SubjectId { get; set; }

    [Display(Name = "Question_Count")]
    public int QuestionCount { get; set; }

    [Display(Name = "Subtopic")]
    public Guid SubtopicId { get; set; }

    [BindProperty]
    public List<AdminExamRuleSubtopicUpdateVM>? ExamRuleSubtopics { get; set; }
}