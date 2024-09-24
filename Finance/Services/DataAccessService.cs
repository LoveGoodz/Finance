using Finance.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Finance.Services
{
    public class DataAccessService : IDataAccessService
    {
        private readonly FinanceContext _context;

        public DataAccessService(FinanceContext context)
        {
            _context = context;
        }

        public async Task<List<T>> GetAllAsync<T>() where T : class
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T> GetByIdAsync<T>(int id) where T : class
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task AddAsync<T>(T entity) where T : class
        {
            await _context.Set<T>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync<T>(T entity) where T : class
        {
            _context.Set<T>().Update(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync<T>(T entity) where T : class
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<List<T>> GetPagedAsync<T>(int pageNumber, int pageSize, Expression<Func<T, bool>> predicate) where T : class
        {
            return await _context.Set<T>()
                                 .Where(predicate)
                                 .Skip((pageNumber - 1) * pageSize)
                                 .Take(pageSize)
                                 .ToListAsync();
        }

        public async Task<int> CountAsync<T>(Expression<Func<T, bool>> predicate) where T : class
        {
            return await _context.Set<T>().CountAsync(predicate);  
        }
    }
}
