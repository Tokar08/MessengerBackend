using System.Linq.Expressions;
using MessangerBackend.Core.Interfaces;

namespace MessangerBackend.Storage;

public class Repository : IRepository
{
    private readonly MessangerContext _context;

    public Repository(MessangerContext context)
    {
        _context = context;
    }

    public async Task<T> Add<T>(T entity) where T : class
    {
        _context.Set<T>().Add(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public Task<T> Update<T>(T entity) where T : class
    {
        throw new NotImplementedException();
    }

    public Task<T> Delete<T>(int id) where T : class
    {
        throw new NotImplementedException();
    }

    public async Task<T> GetById<T>(int id) where T : class
    {
        return await _context.Set<T>().FindAsync(id);
    }

    public IQueryable<T> GetAll<T>() where T : class
    {
        return _context.Set<T>().AsQueryable();
    }

    public async Task<IEnumerable<T>> GetQuery<T>(Expression<Func<T, bool>> func) where T : class
    {
        return _context.Set<T>().Where(func);
    }
}