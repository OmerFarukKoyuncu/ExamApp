using BAExamApp.Business.Services;
using BAExamApp.Dtos.BreadcrumbItem;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Business.Interfaces.Services;
public interface IBreadcrumbService
{
    List<BreadcrumbItemDto> GenerateBreadcrumbs(string area, string controller, string action, bool isHomePage, bool isLoginPage);
    (string Name,bool IsActive) GetDisplayName(string areaName, string controllerName, string actionName);
    void SwitchToManual();
    void SwitchToAuto();
    void AddBreadcrumb(BreadcrumbItemDto breadcrumb);
}