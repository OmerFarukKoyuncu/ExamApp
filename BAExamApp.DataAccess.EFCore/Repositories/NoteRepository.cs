namespace BAExamApp.DataAccess.EFCore.Repositories;
public class NoteRepository : EFBaseRepository<Note>, INoteRepository
{
    public NoteRepository(BAExamAppDbContext context) : base(context)
    {
    }
}
