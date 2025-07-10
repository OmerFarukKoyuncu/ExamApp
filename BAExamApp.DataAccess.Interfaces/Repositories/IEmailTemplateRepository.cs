using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.DataAccess.Interfaces.Repositories;
public interface IEmailTemplateRepository : IRepository, IAsyncQueryableRepository<EmailTemplate>, IAsyncFindableRepository<EmailTemplate>, IAsyncUpdateableRepository<EmailTemplate>, IAsyncInsertableRepository<EmailTemplate>, IAsyncDeleteableRepository<EmailTemplate>
{
}
