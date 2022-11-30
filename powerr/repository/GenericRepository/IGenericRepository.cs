using powerr.Models.Entities.Meter;
using System.Linq.Expressions;
using System.Security.Principal;

namespace powerr.repository.GenericRepository
{
  
        public interface IGenericRepository<TEntity> where TEntity : class 
        {
            IQueryable<TEntity> GetAll();

            Task<TEntity> GetByCondition(Expression<Func<TEntity, bool>> predicate);

            Task Create(TEntity entity);

            Task Update(int id, TEntity entity);

            Task Delete(int id);
        }
    }

