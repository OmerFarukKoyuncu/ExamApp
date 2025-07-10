using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Dtos.EmailTemplateDtos;
public class EmailTemplateListDto
{
    public Guid Id { get; set; }
    public string ModelName { get; set; }
    public string Subject { get; set; }
    public string Template { get; set; }
}
