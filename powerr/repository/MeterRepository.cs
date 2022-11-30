using Microsoft.EntityFrameworkCore;
using powerr.Api.repository;
using powerr.Interfaces;
using powerr.Models.Dtos;
using powerr.Models.Entities.Meter;
using System.Linq.Expressions;

namespace powerr.repository
{
    public class MeterRepository : IMeterRepository
    {
        private RepositoryContext _context;

        public MeterRepository(RepositoryContext context)
        {
            _context = context;
        }


        public void Create(Meter meter)
        {
            _context.meters.Add(meter);
        }

        public void Delete(int meterId)
        {
            throw new NotImplementedException();
        }

        public async Task<Meter> FindByConditionAsync(Expression<Func<Meter, bool>> predicate)
        {
            return await _context.meters.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<Meter>> GetAll()
        {
            return await _context.Set<Meter>().ToListAsync();
        }

        public async Task<Meter> GetById(int meterId)
        {
            return await _context.Set<Meter>().FirstOrDefaultAsync(m => m.Id == meterId);
            
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Update(Meter meterRequest)
        {
            _context.Set<Meter>().Update(meterRequest);
        }
    }
}
