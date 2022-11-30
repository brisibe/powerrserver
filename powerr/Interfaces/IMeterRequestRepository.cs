using Microsoft.AspNetCore.Mvc;
using powerr.Models.Dtos;
using powerr.Models.Entities.Meter;
using System.Linq.Expressions;

namespace powerr.Interfaces
{
    public interface IMeterRequestRepository
    {
        Task<IEnumerable<MeterRequest>> GetAll();
        MeterRequest GetByMeterId(string meterId);

        Task<MeterRequest> FindByConditionAsync(Expression<Func<MeterRequest, bool>> predicate);

        void Create(MeterRequest meterRequest);
        void Update(MeterRequest meterRequest);
        void Delete(int meterId);

        void Save();

    }
}
