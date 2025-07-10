using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Dtos.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false)]
public class BreadcrumbNameAttribute : Attribute
{
    public string Name { get; }
    public bool IsActive { get; }
    
    public BreadcrumbNameAttribute(string name, bool isActive = true)
    {
        Name = name;
        IsActive = isActive;
    }
}