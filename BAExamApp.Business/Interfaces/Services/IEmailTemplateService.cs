using BAExamApp.Dtos.Emails;
using BAExamApp.Dtos.EmailTemplateDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Business.Interfaces.Services;
public interface IEmailTemplateService
{
    Task<bool> AnyAsync(Expression<Func<EmailTemplate, bool>> expression);
    Task<IDataResult<EmailTemplateDto>> GetById(Guid Id);
    Task<IDataResult<List<EmailTemplateListDto>>> GetAllAsync();
    Task<IDataResult<EmailTemplateDto>> AddAsync(EmailTemplateCreateDto emailTemplateCreateDto);
    Task<IDataResult<List<EmailTemplateDto>>> AddRangeAsync(List<EmailTemplateCreateDto> emailTemplatesCreateDto);
    Task<IDataResult<EmailTemplateDto>> UpdateAsync(EmailTemplateUpdateDto emailTemplateUpdateDto);
    Task<IDataResult<List<EmailTemplateDto>>> UpdateRangeAsync(List<EmailTemplateCreateDto> emailTemplatesCreateDto, string modelName);
    Task<IResult> DeleteAsync(Guid id);
    Task<IResult> DeleteRangeAsync(List<Guid> ids);
}
