using BAExamApp.Core.Utilities.Results;

namespace BAExamApp.DataAccess.Interfaces.Repositories;

public interface ITrainerClassroomRepository : IAsyncRepository, IAsyncInsertableRepository<TrainerClassroom>, IAsyncFindableRepository<TrainerClassroom>, IAsyncDeleteableRepository<TrainerClassroom>, IAsyncUpdateableRepository<TrainerClassroom>, IAsyncTransactionRepository
{


    Task AddAsync(TrainerClassroom trainerClassroom);

}
