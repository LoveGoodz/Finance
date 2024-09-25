using System.Linq.Expressions;

public interface IDataAccessService
{
    Task<List<T>> GetAllAsync<T>() where T : class;
    Task<T> GetByIdAsync<T>(int id) where T : class;
    Task AddAsync<T>(T entity) where T : class;
    Task UpdateAsync<T>(T entity) where T : class;
    Task DeleteAsync<T>(T entity) where T : class;
    Task<List<T>> GetPagedAsync<T>(int pageNumber, int pageSize, Expression<Func<T, bool>> predicate) where T : class;
    Task<int> CountAsync<T>(Expression<Func<T, bool>> predicate) where T : class;  
}
