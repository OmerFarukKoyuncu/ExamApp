using BAExamApp.Dtos.Attributes;
using BAExamApp.Dtos.BreadcrumbItem;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Reflection;

public class BreadcrumbService : IBreadcrumbService
{
    private readonly Assembly _mvcAssembly;

    private List<BreadcrumbItemDto> _breadcrumbs;
    private bool _isManual = false;

    public BreadcrumbService()
    {
        // Uygulama başlangıcında assembly yüklemesi yapılır.
        _mvcAssembly = Assembly.Load("BAExamApp.MVC");
        _breadcrumbs = new List<BreadcrumbItemDto>();
    }

    public List<BreadcrumbItemDto> GenerateBreadcrumbs(string area, string controller, string action, bool isHomePage, bool isLoginPage)
    {
        // Eğer giriş ekranı (isLoginPage) veya ana sayfa (isHomePage) ise, breadcrumb'ı hiç eklemeyiz.
        if (isLoginPage || isHomePage)
        {
            return new List<BreadcrumbItemDto>();
        }
        
        //Anasayfa default breadcrumb ekle
        var homeBreadcrumb = !string.IsNullOrEmpty(area)
            ? new BreadcrumbItemDto { Title = "Index", Url = $"/{area}", IsActive = true }
            : new BreadcrumbItemDto { Title = "Index", Url = "/", IsActive = true };

        // Eğer manuel mod değilse listeyi sıfırla.
        if (!_isManual)
            _breadcrumbs = new List<BreadcrumbItemDto>();
        
        // İlk sıraya anasayfa ekle.
        _breadcrumbs.Insert(0, homeBreadcrumb);
        
        // Eğer manual ise direkt listeyi dön.
        if (_isManual) return _breadcrumbs;

        // Eğer controller varsa, breadcrumb listesine eklenir.
        if (!string.IsNullOrEmpty(controller))
        {
            var result = GetDisplayName(area, controller, null); // Controller için BreadcrumbName al
            _breadcrumbs.Add(new BreadcrumbItemDto { Title = result.Name, Url = $"/{area}/{controller}", IsActive = result.IsActive});
        }

        // Eğer action varsa ve action "Index" değilse, breadcrumb listesine eklenir.
        if (!string.IsNullOrEmpty(action) && !action.Equals("Index", StringComparison.OrdinalIgnoreCase))
        {
            var result = GetDisplayName(area, controller, action); // Action için BreadcrumbName al
            _breadcrumbs.Add(new BreadcrumbItemDto { Title = result.Name, Url = $"/{area}/{controller}/{action}", IsActive = result.IsActive});
        }

        return _breadcrumbs;
    }

    public (string Name,bool IsActive) GetDisplayName(string areaName, string controllerName, string actionName)
    {
        // Controller'ı Area bilgisine göre filtreleyerek bul
        var controllerType = _mvcAssembly.GetTypes()
            .FirstOrDefault(t =>
                t.Name.Equals($"{controllerName}Controller", StringComparison.OrdinalIgnoreCase) &&
                (string.IsNullOrEmpty(areaName) ||
                 t.GetCustomAttribute<AreaAttribute>()?.RouteValue.Equals(areaName, StringComparison.OrdinalIgnoreCase) == true)
            );

        if (controllerType == null) return (controllerName, true);

        // Eğer action adı verilmemişse sadece controller'ı döndür
        if (string.IsNullOrEmpty(actionName))
        {
            var controllerAttribute = controllerType.GetCustomAttribute<BreadcrumbNameAttribute>();
            return (controllerAttribute?.Name ?? controllerName, controllerAttribute?.IsActive ?? true);
        }

        // Action metodunu bul
        var actionMethod = controllerType.GetMethods()
            .FirstOrDefault(m => m.Name.Equals(actionName, StringComparison.OrdinalIgnoreCase));

        if (actionMethod == null) return (actionName, true);

        var actionAttribute = actionMethod.GetCustomAttribute<BreadcrumbNameAttribute>();
        return (actionAttribute?.Name ?? actionName, actionAttribute?.IsActive ?? true);
    }

    public void SwitchToManual()
    {
        _isManual = true;
    }

    public void SwitchToAuto()
    {
        _isManual = false;
    }

    public void AddBreadcrumb(BreadcrumbItemDto breadcrumb)
    {
        _breadcrumbs.Add(breadcrumb);
    }
}