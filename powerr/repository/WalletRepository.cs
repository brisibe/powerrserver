using Microsoft.EntityFrameworkCore;
using powerr.Api.repository;
using powerr.Interfaces;
using powerr.Models.Entities.Wallet;
using System.Linq.Expressions;

namespace powerr.repository
{
    public class WalletRepository : IWalletRepository
    {

        private readonly RepositoryContext _context;

        public WalletRepository(RepositoryContext context)
        {
            _context = context;
        }

        public void Create(Wallet wallet)
        {
            _context.wallets.Add(wallet);
        }

        public async Task<Wallet> FindByConditionAsync(Expression<Func<Wallet, bool>> predicate)
        {
            return await _context.wallets.FirstOrDefaultAsync(predicate);
        }

       
        public void Save()
        {
            _context.SaveChanges();
        }

        public void Update(Wallet wallet)
        {
            _context.Update(wallet);
        }
    }
}
