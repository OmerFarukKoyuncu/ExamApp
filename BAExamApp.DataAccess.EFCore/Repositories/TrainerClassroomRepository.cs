using BAExamApp.Core.Utilities.Results;

namespace BAExamApp.DataAccess.EFCore.Repositories;

public class TrainerClassroomRepository : EFBaseRepository<TrainerClassroom>, ITrainerClassroomRepository
{
    private readonly BAExamAppDbContext _context;

    public TrainerClassroomRepository(BAExamAppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task AddAsync(TrainerClassroom entity)
    {
        await _context.Trainers_Classrooms.AddAsync(entity);
        await _context.SaveChangesAsync();
    }
}
