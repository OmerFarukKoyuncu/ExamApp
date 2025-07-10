using BAExamApp.Dtos.ApiDtos.GroupTypeApiDtos;
using BAExamApp.Dtos.GroupTypes;

namespace BAExamApp.Business.Profiles;

public class GroupTypeProfile : Profile
{
    public GroupTypeProfile()
    {
        CreateMap<GroupType, GroupTypeDto>()
             .ForMember(dest => dest.Classrooms, opt => opt.MapFrom(src => src.Classrooms.Where(x => x.Status != Core.Enums.Status.Deleted)));
        CreateMap<GroupType, GroupTypeListDto>();
        CreateMap<GroupTypeCreateDto, GroupType>()
            .ForMember(dest => dest.Name,
            config => config.MapFrom(src => src.Name.Trim()));
        CreateMap<GroupTypeUpdateDto, GroupType>()
            .ForMember(dest => dest.Name,
            config => config.MapFrom(src => src.Name.Trim()));
        CreateMap<GroupType, GroupTypeListApiDto>();

        CreateMap<GroupType, GroupTypeApiDto>();
        CreateMap<GroupTypeCreateApiDto, GroupType>()
            .ForMember(dest => dest.Name, 
            config => config.MapFrom(src => src.Name.Trim()));

        CreateMap<GroupTypeUpdateApiDto, GroupType>(); 
        CreateMap<GroupType, GroupTypeApiDto>();
    }
}