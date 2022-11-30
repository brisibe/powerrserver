//using Microsoft.EntityFrameworkCore;
//using powerr.Api.repository;
//using System.Security.Principal;

//namespace powerr.repository.GenericRepository
//{
//    public class GenericRepository<TEntity> : IGenericRepository<TEntity>
//     where TEntity : class 
//    {
//        private readonly RepositoryContext _context;

//        public GenericRepository(RepositoryContext context)
//        {
//            _context = context;
//        }


//        public IQueryable<TEntity> GetAll()
//        {
//            return _context.Set<TEntity>().AsNoTracking();
//        }

//        public async Task<TEntity> GetById(int id)
//        {
//            return await _context.Set<TEntity>()
//                        .AsNoTracking()
//                        .FirstOrDefaultAsync(e =>  e.Id == id );
//        }
//    }
//}
