using powerr.Models.Entities.Meter;
using System.Linq.Expressions;

namespace powerr.Interfaces
{
    public interface IMeterRepository
    {
        Task<IEnumerable<Meter>> GetAll();
        Task<Meter> GetById(int meterId);

        Task<Meter> FindByConditionAsync(Expression<Func<Meter, bool>> predicate);

        void Create(Meter meterRequest);
        void Update(Meter meterRequest);
        void Delete(int meterId);

        void Save();
    }
}
