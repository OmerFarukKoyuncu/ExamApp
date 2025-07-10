using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.DataAccess.EFCore.Repositories;
public class EmailTemplateRepository : EFBaseRepository<EmailTemplate>, IEmailTemplateRepository
{
    public EmailTemplateRepository(BAExamAppDbContext context) : base(context)
    {

    }

}
