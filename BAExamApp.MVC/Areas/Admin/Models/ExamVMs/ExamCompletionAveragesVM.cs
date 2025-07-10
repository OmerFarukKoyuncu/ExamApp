namespace BAExamApp.MVC.Areas.Admin.Models.ExamVMs;

public class ExamCompletionAveragesVM
{
    public Guid ClassroomId { get; set; }
    public string? ClassroomName { get; set; }
    public TimeSpan AverageCompletionTime { get; set; }
}
