using BAExamApp.Dtos.QuestionArranges;

namespace BAExamApp.Business.Profiles;
internal class QuestionArrangeProfile : Profile
{
    public QuestionArrangeProfile()
    {
        CreateMap<QuestionRevision, QuestionArrangeCreateDto>().ForMember(dest => dest.ArrangerAdminId, opt => opt.MapFrom(src => src.RequesterAdminId)).ReverseMap();
        CreateMap<QuestionArrangeDto, QuestionArrangeCreateDto>().ReverseMap();
        CreateMap<QuestionArrangeListDto, QuestionRevision>().ForMember(dest => dest.RequesterAdminId, opt => opt.MapFrom(src => src.ArrangerAdminId)).ReverseMap();
    }
}
