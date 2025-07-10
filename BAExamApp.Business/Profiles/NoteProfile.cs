using BAExamApp.Dtos.Dashboard;
using BAExamApp.Dtos.Notes;

namespace BAExamApp.Business.Profiles;
public class NoteProfile : Profile
{
    public NoteProfile()
    {
        CreateMap<Note, NoteDto>();
        CreateMap<NoteCreateDto, Note>();
        CreateMap<DashboardNoteDto, Note>().ReverseMap();
    }
}
