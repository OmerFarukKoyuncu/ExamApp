using BAExamApp.Dtos.Notes;

namespace BAExamApp.Business.Interfaces.Services;
public interface INoteService
{
    Task<IDataResult<NoteDto>> AddAsync(NoteCreateDto noteCreateDto);
    Task<IDataResult<string>> DeleteAsync(Guid id);
    Task<IDataResult<NoteDto>> GetByIdAsync(Guid id);
}
