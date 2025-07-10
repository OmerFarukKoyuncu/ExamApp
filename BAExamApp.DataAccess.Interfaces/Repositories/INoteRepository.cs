namespace BAExamApp.DataAccess.Interfaces.Repositories;
public interface INoteRepository: IAsyncRepository, IAsyncQueryableRepository<Note>, IAsyncFindableRepository<Note>, IAsyncInsertableRepository<Note>, IAsyncUpdateableRepository<Note>, IAsyncDeleteableRepository<Note>
{
}
