using System.ComponentModel.DataAnnotations;

namespace BAExamApp.MVC.Areas.Student.Models.ExamVMs;

public class StudentExamListVM
{
    public Guid Id { get; set; }

    [Display(Name = "Exam_Name")]
    public string ExamName { get; set; }

    [Display(Name = "Exam_Date_Time")]
    public DateTime ExamDateTime { get; set; }

    [Display(Name = "Exam_Duration")]
    public TimeSpan ExamDuration { get; set; }

    [Display(Name = "Score")]
    public decimal? Score { get; set; }

    [Display(Name = "Answered_Question_Count")]
    public int AnsweredQuestionCount { get; set; } = 0;

    [Display(Name = "Start_Time")]
    public DateTime? StartTime { get; set; }

    [Display(Name = "End_Time")]
    public DateTime? EndTime { get; set; }
}