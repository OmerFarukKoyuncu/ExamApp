using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Dtos.BreadcrumbItem;
public class BreadcrumbItemDto
{
    public string Title { get; set; } // Örn: Eğitmenler
    public string Url { get; set; }   // Örn: /Admin/Trainer
    public bool IsActive { get; set; } // Aktif olan sayfa için true
}