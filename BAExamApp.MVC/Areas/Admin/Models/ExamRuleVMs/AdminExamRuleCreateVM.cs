using BAExamApp.Entities.Enums;
using BAExamApp.MVC.Areas.Admin.Models.ExamRuleSubtopicVMs;
using System.ComponentModel.DataAnnotations;

namespace BAExamApp.MVC.Areas.Admin.Models.ExamRuleVMs;

public class AdminExamRuleCreateVM
{
    [Display(Name = "Exam_Rule_Name")]
    [Required(ErrorMessage = "Required_Field_Error")]
    public string Name { get; set; }

    [Display(Name = "Exam_Type")]
    [Required( ErrorMessage = "Required_Field_Error")]
    public ExamType ExamType { get; set; }

    [Display(Name = "Exam_Rule_Description")]
    [Required(ErrorMessage = "Required_Field_Error")]
    public string? Description { get; set; }

    [Display(Name = "Question_Type")]
    [Required(ErrorMessage = "Required_Field_Error")]
    public QuestionType QuestionType { get; set; }

    [Display(Name = "Question_Difficulty")]
    [Required(ErrorMessage = "Required_Field_Error")]
    public Guid QuestionDifficultyId { get; set; }

    [Display(Name = "Product")]
    public Guid ProductId { get; set; }
    
    [Display(Name = "Subject")]
    [Required(ErrorMessage = "Required_Field_Error")]
    public Guid SubjectId { get; set; }

    [Display(Name = "Subtopic")]
    public Guid SubtopicId { get; set; }

    [Display(Name = "Question_Count")]
    [Required(ErrorMessage = "Required_Field_Error")]
    public int QuestionCount { get; set; }

    [BindProperty]
    public List<AdminExamRuleSubtopicCreateVM>? ExamRuleSubtopics { get; set; }
}