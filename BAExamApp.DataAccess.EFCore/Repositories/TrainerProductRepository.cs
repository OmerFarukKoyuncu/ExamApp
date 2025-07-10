using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAExamApp.DataAccess.EFCore.Repositories;
public class TrainerProductRepository : EFBaseRepository<TrainerProduct>, ITrainerProductRepository
{
    private readonly BAExamAppDbContext _context;

    public TrainerProductRepository(BAExamAppDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task AddAsync(TrainerProduct trainerProduct)
    {
        await _context.Trainers_Products.AddAsync(trainerProduct);
        await _context.SaveChangesAsync();
    }
}
