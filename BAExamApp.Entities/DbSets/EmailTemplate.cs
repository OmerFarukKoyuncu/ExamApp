using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Entities.DbSets;
public class EmailTemplate : AuditableEntity
{
    public string ModelName { get; set; }
    public string Subject { get; set; }
    public string Template { get; set; }
}
