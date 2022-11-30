using Microsoft.EntityFrameworkCore;
using powerr.Api.repository;
using powerr.Interfaces;
using powerr.Models.Entities.MeterToken;
using System.Linq.Expressions;

namespace powerr.repository
{
    public class RechargeTokenRepository : IRechargeTokenRepository
    {

        private readonly RepositoryContext _context;

        public RechargeTokenRepository(RepositoryContext context)
        {
            _context = context;
        }

          public void Create(RechargeToken rechargeTok)
        {
            _context.rechargeTokens.Add(rechargeTok);
        }

        public async Task<RechargeToken> FindByConditionAsync(Expression<Func<RechargeToken, bool>> predicate)
        {
            return await _context.rechargeTokens.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<RechargeToken>> GetAll()
        {
            return await _context.rechargeTokens.ToListAsync();
        }

        public Task<RechargeToken> GetByToken(long token)
        {
            throw new NotImplementedException();
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Update(RechargeToken rechargeToken)
        {
            _context.Update(rechargeToken);
        }
    }
}
