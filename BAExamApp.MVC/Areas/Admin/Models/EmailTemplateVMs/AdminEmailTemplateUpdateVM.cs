using BAExamApp.Dtos.SendMails;
using System.Reflection;

namespace BAExamApp.MVC.Areas.Admin.Models.EmailTemplateVMs;

public class AdminEmailTemplateUpdateVM
{
    public AdminEmailTemplateUpdateVM()
    {
        var typeOfPathfinder = typeof(PathFinderForDTOs);
        var types = Assembly.GetAssembly(typeOfPathfinder).GetTypes().Where(x => x.Namespace == typeOfPathfinder.Namespace && !x.IsAbstract && !x.IsInterface).ToList();
        SendMailDTOModels = types;
    }
    public Guid Id { get; set; }
    public string ModelName { get; set; }
    public string Subject { get; set; }
    public string Template { get; set; }
    public List<Type> SendMailDTOModels { get; set; }
}
