

using BAExamApp.Dtos.EmailTemplateDtos;

namespace BAExamApp.Business.Profiles;
public class EmailTemplateProfile : Profile
{
    public EmailTemplateProfile()
    {
        CreateMap<EmailTemplate, EmailTemplateDto>().ReverseMap();
        CreateMap<EmailTemplate, EmailTemplateCreateDto>().ReverseMap();
        CreateMap<EmailTemplate, EmailTemplateDetailsDto>().ReverseMap();
        CreateMap<EmailTemplate, EmailTemplateUpdateDto>().ReverseMap();
        CreateMap<EmailTemplate, EmailTemplateListDto>().ReverseMap();
        CreateMap<EmailTemplateDto, EmailTemplateDetailsDto>().ReverseMap();
        CreateMap<EmailTemplateDto, EmailTemplateUpdateDto>().ReverseMap();
        CreateMap<EmailTemplateDto, EmailTemplateListDto>().ReverseMap();
        CreateMap<EmailTemplateDto, EmailTemplateCreateDto>().ReverseMap();
    }
}
