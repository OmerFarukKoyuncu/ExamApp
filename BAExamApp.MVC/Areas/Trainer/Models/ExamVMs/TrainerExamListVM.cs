using BAExamApp.Entities.Enums;
using Elfie.Serialization;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace BAExamApp.MVC.Areas.Trainer.Models.ExamVMs;

public class TrainerExamListVM
{
    public Guid Id { get; set; }

    [Display(Name ="Exam_Name", ResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    public string Name { get; set; }

    [Display(Name = "Exam_Date", ResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    public DateTime ExamDateTime { get; set; } = DateTime.Now;

    [Display(Name = "Exam_Duration", ResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    public TimeSpan ExamDuration { get; set; }

    [Display(Name = "Classroom", ResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    public string ClassroomName { get; set; }
    [Display(Name = "Exam_Type", ResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    public ExamType ExamType { get; set; }
    [Display(Name= "Question_CreatedDate", ResourceType = typeof(BAExamApp.MVC.Resources.SharedModelResource))]
    public DateTime CreatedDate { get; set; }
    public bool IsStarted { get; set; }
    

}