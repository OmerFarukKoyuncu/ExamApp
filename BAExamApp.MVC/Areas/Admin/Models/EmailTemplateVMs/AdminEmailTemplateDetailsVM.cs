namespace BAExamApp.MVC.Areas.Admin.Models.EmailTemplateVMs;

public class AdminEmailTemplateDetailsVM
{
    public Guid Id { get; set; }
    public string ModelName { get; set; }
    public string Subject { get; set; }
    public string Template { get; set; }
}
