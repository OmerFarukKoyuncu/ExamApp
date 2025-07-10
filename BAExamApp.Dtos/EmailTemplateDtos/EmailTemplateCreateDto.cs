using BAExamApp.Dtos.SendMails;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Dtos.EmailTemplateDtos;
public class EmailTemplateCreateDto
{
    public string ModelName { get; set; }
    public string Subject { get; set; }
    public string Template { get; set; }
}
