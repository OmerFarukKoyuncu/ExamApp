using BAExamApp.Core.Enums;
using BAExamApp.DataAccess.Interfaces.Repositories;
using BAExamApp.Dtos.Branches;
using BAExamApp.Dtos.Dashboard;
using BAExamApp.Dtos.Notes;

namespace BAExamApp.Business.Services;
public class NoteService : INoteService
{
    private readonly INoteRepository _noteRepository;
    private readonly IMapper _mapper;
    public NoteService(INoteRepository noteRepository, IMapper mapper)
    {
        _noteRepository = noteRepository;
        _mapper = mapper;
    }
    public async Task<IDataResult<NoteDto>> AddAsync(NoteCreateDto noteCreateDto)
    {
        var note = _mapper.Map<Note>(noteCreateDto);

        await _noteRepository.AddAsync(note);
        await _noteRepository.SaveChangesAsync();

        return new SuccessDataResult<NoteDto>(_mapper.Map<NoteDto>(note), Messages.AddSuccess);
    }

    public async Task<IDataResult<NoteDto>> GetByIdAsync(Guid id)
    {
        var note = await _noteRepository.GetByIdAsync(id);

        if (note is null)
        {
            return new ErrorDataResult<NoteDto>(Messages.BranchNotFound);
        }

        return new SuccessDataResult<NoteDto>(_mapper.Map<NoteDto>(note), Messages.FoundSuccess);
    }

    public async Task<IDataResult<string>> DeleteAsync(Guid id)
    {

        var note = await _noteRepository.GetByIdAsync(id);

        if (note is null)
        {
            return new ErrorDataResult<string>(Messages.DeleteFail);
        }

        await _noteRepository.DeleteAsync(note);
        await _noteRepository.SaveChangesAsync();

        return new SuccessDataResult<string>( Messages.DeleteSuccess);


    }


}
