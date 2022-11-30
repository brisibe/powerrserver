using powerr.Models.Entities.MeterToken;
using System.Linq.Expressions;

namespace powerr.Interfaces
{
    public interface IRechargeTokenRepository
    {
        Task<IEnumerable<RechargeToken>> GetAll();
        Task<RechargeToken> GetByToken(long token);

        Task<RechargeToken> FindByConditionAsync(Expression<Func<RechargeToken, bool>> predicate);

        void Create(RechargeToken rechargeTok);
        void Update(RechargeToken rechargeToken);
        //void Delete(int meterId);

        void Save();
    }
}
