using System.Linq.Expressions;

namespace BAExamApp.Core.DataAccess.Interfaces;

public interface IAsyncPaginateRepository<TEntity> : IAsyncQueryableRepository<TEntity>, IAsyncRepository where TEntity : BaseEntity
{
    Task<IEnumerable<TEntity>> GetAllAsync<TKey>(Expression<Func<TEntity, bool>> expression, Expression<Func<TEntity, TKey>> orderby, int skip, int take, bool orderDesc = false, bool tracking = true);
}
