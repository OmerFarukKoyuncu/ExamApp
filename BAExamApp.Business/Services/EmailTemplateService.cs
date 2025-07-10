using BAExamApp.DataAccess.Interfaces.Repositories;
using BAExamApp.Dtos.Emails;
using BAExamApp.Dtos.EmailTemplateDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.Business.Services;
public class EmailTemplateService : IEmailTemplateService
{
    private readonly IEmailTemplateRepository _emailTemplateRepository;
    private readonly IMapper _mapper;

    public EmailTemplateService(IEmailTemplateRepository emailTemplateRepository, IMapper mapper)
    {
        _emailTemplateRepository = emailTemplateRepository;
        _mapper = mapper;
    }

    public async Task<bool> AnyAsync(Expression<Func<EmailTemplate, bool>> expression)
    {
        return await _emailTemplateRepository.AnyAsync(expression);
    }
    public async Task<IDataResult<EmailTemplateDto>> GetById(Guid Id)
    {
        var email = await _emailTemplateRepository.GetByIdAsync(Id);
        if (email == null)
        {
            return new ErrorDataResult<EmailTemplateDto>(Messages.EmailNotFound);
        }
        return new SuccessDataResult<EmailTemplateDto>(_mapper.Map<EmailTemplateDto>(email), Messages.EmailFoundSuccess);
    }
    public async Task<IDataResult<List<EmailTemplateListDto>>> GetAllAsync()
    {
        var emailTemplates = await _emailTemplateRepository.GetAllAsync();

        if (emailTemplates != null)
        {
            return new SuccessDataResult<List<EmailTemplateListDto>>(_mapper.Map<List<EmailTemplateListDto>>(emailTemplates), Messages.EmailFoundSuccess);
        }

        return new ErrorDataResult<List<EmailTemplateListDto>>(Messages.EmailNotFound);
    }

    public async Task<IDataResult<EmailTemplateDto>> AddAsync(EmailTemplateCreateDto emailTemplateCreateDto)
    {
        var hasModel = await _emailTemplateRepository.AnyAsync(x => x.ModelName.Trim().ToLower() == emailTemplateCreateDto.ModelName.Trim().ToLower());

        if (hasModel)
        {
            return new ErrorDataResult<EmailTemplateDto>(Messages.AddFailAlreadyExists);
        }

        var emailTemplate = _mapper.Map<EmailTemplate>(emailTemplateCreateDto);

        await _emailTemplateRepository.AddAsync(emailTemplate);
        await _emailTemplateRepository.SaveChangesAsync();

        return new SuccessDataResult<EmailTemplateDto>(_mapper.Map<EmailTemplateDto>(emailTemplate), Messages.AddSuccess);
    }

    public async Task<IDataResult<List<EmailTemplateDto>>> AddRangeAsync(List<EmailTemplateCreateDto> emailTemplateCreateDtos)
    {
        var emailTemplates = new List<EmailTemplate>();
        var uniqueModelsCreateDtoList = emailTemplateCreateDtos
        .Where(x => !string.IsNullOrEmpty(x.ModelName))
        .GroupBy(x => x.ModelName)
        .Select(group => group.First())
        .ToList();

        foreach (var uniqueEmailCreateDto in uniqueModelsCreateDtoList)
        {
            var emailTemplate = _mapper.Map<EmailTemplate>(uniqueEmailCreateDto);

            await _emailTemplateRepository.AddAsync(emailTemplate);

            emailTemplates.Add(emailTemplate);
        }
        await _emailTemplateRepository.SaveChangesAsync();

        return new SuccessDataResult<List<EmailTemplateDto>>(_mapper.Map<List<EmailTemplateDto>>(emailTemplates), Messages.AddSuccess);
    }

    public async Task<IDataResult<EmailTemplateDto>> UpdateAsync(EmailTemplateUpdateDto emailTemplateUpdateDto)
    {
        var emailTemplate = await _emailTemplateRepository.GetByIdAsync(emailTemplateUpdateDto.Id);

        if (emailTemplate is null)
        {
            return new ErrorDataResult<EmailTemplateDto>(Messages.EmailNotFound);
        }

        var updatedEmailTemplate = _mapper.Map(emailTemplateUpdateDto, emailTemplate);

        await _emailTemplateRepository.UpdateAsync(updatedEmailTemplate);
        await _emailTemplateRepository.SaveChangesAsync();

        return new SuccessDataResult<EmailTemplateDto>(_mapper.Map<EmailTemplateDto>(updatedEmailTemplate), Messages.UpdateSuccess);
    }

    public async Task<IDataResult<List<EmailTemplateDto>>> UpdateRangeAsync(List<EmailTemplateCreateDto> emailTemplatesCreateDto, string modelName)
    {

        var currentEmailTemplates = await _emailTemplateRepository.GetAllAsync(x => x.ModelName == modelName);
        await DeleteRangeAsync(currentEmailTemplates.Select(x => x.Id).ToList());


        return await AddRangeAsync(emailTemplatesCreateDto);
    }

    public async Task<IResult> DeleteAsync(Guid id)
    {
        var emailTemplate = await _emailTemplateRepository.GetByIdAsync(id);

        if (emailTemplate is null)
        {
            return new ErrorDataResult<EmailTemplateDto>(Messages.EmailNotFound);
        }

        await _emailTemplateRepository.DeleteAsync(emailTemplate);
        await _emailTemplateRepository.SaveChangesAsync();

        return new SuccessResult(Messages.DeleteSuccess);
    }

    public async Task<IResult> DeleteRangeAsync(List<Guid> ids)
    {
        foreach (var id in ids)
        {
            var emailTemplate = await _emailTemplateRepository.GetByIdAsync(id);

            if (emailTemplate is null)
            {
                return new ErrorDataResult<EmailTemplateDto>(Messages.EmailNotFound);
            }

            await _emailTemplateRepository.DeleteAsync(emailTemplate);
        }

        await _emailTemplateRepository.SaveChangesAsync();

        return new SuccessResult(Messages.DeleteSuccess);
    }
}
