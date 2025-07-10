using Microsoft.Extensions.Localization;

namespace BAExamApp.MVC.Components
{
    public class BreadcrumbViewComponent : ViewComponent
    {
        private readonly IBreadcrumbService _breadcrumbService;
        private readonly IStringLocalizer<SharedModelResource> _localizer;

        public BreadcrumbViewComponent(IBreadcrumbService breadcrumbService, IStringLocalizer<SharedModelResource> localizer)
        {
            _breadcrumbService = breadcrumbService;
            _localizer = localizer;
        }

        public IViewComponentResult Invoke()
        {
            var area = RouteData.Values["area"]?.ToString() ?? string.Empty;
            var controller = RouteData.Values["controller"]?.ToString() ?? string.Empty;
            var action = RouteData.Values["action"]?.ToString() ?? string.Empty;

            // Ana sayfa ve giriş ekranı kontrolü
            bool isHomePage = controller.Equals("Home", StringComparison.OrdinalIgnoreCase) && action.Equals("Index", StringComparison.OrdinalIgnoreCase);
            bool isLoginPage = controller.Equals("Account", StringComparison.OrdinalIgnoreCase) && action.Equals("Login", StringComparison.OrdinalIgnoreCase);

            // Breadcrumb'ları oluştururken, giriş ve ana sayfa ekranlarında breadcrumb gösterilmesin
            var breadcrumbs = _breadcrumbService.GenerateBreadcrumbs(area, controller, action, isHomePage, isLoginPage);

            // Breadcrumb öğelerinin başlıklarını localize etme ve son öğeyi pasif yapma
            for (int i = 0; i < breadcrumbs.Count; i++)
            {
                var breadcrumb = breadcrumbs[i];
                breadcrumb.Title = _localizer[breadcrumb.Title]; // Başlığı localize et

                // Son öğeyi pasif hale getir
                if (i == breadcrumbs.Count - 1)
                {
                    breadcrumb.Url = null; // URL'yi boş bırakarak pasif yapıyoruz
                }
            }

            return View(breadcrumbs);
        }
    }
}
