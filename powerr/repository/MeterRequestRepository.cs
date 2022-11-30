using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using powerr.Api.repository;
using powerr.Interfaces;
using powerr.Models.Dtos;
using powerr.Models.Entities.Meter;
using System.Linq.Expressions;

namespace powerr.repository
{
    public class MeterRequestRepository : IMeterRequestRepository
    {
        private RepositoryContext _context;
        public MeterRequestRepository(RepositoryContext context)
        {
            _context = context;

        }
        public  void Create(MeterRequest meterRequest)
        {
            _context.meterRequests.AddAsync(meterRequest);
        }

        public void Delete(int meterId)
        {
            throw new NotImplementedException();
        }

        public async Task<MeterRequest> FindByConditionAsync(Expression<Func<MeterRequest, bool>> predicate)
        {
            return await _context.meterRequests.Include(m => m.Meters).FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<MeterRequest>> GetAll()
        {

            return await _context.meterRequests.Include(m => m.Meters).ToListAsync();
            
        }

        public MeterRequest GetByMeterId(string meterId)
        {
            return _context.meterRequests.Find(meterId);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Update(MeterRequest meterRequest)
        {
            _context.meterRequests.Update(meterRequest);
            
        }
    }
}
