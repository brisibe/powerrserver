using powerr.Models.Entities.Wallet;
using System.Linq.Expressions;

namespace powerr.Interfaces
{
    public interface IWalletRepository
    {
        Task<Wallet> FindByConditionAsync(Expression<Func<Wallet, bool>> predicate);

        void Create(Wallet wallet);
        void Update(Wallet wallete);
        //void Delete(int walletId);

        void Save();
    }
}
