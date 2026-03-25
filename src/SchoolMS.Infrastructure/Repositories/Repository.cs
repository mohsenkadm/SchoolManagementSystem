using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SchoolMS.Domain.Entities;
using SchoolMS.Domain.Interfaces;
using SchoolMS.Infrastructure.Data;

namespace SchoolMS.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly SchoolDbContext _context;
    protected readonly DbSet<T> _dbSet;
    private readonly ITenantProvider _tenantProvider;

    public Repository(SchoolDbContext context, ITenantProvider tenantProvider)
    {
        _context = context;
        _dbSet = context.Set<T>();
        _tenantProvider = tenantProvider;
    }

    /// <summary>
    /// Returns a queryable with explicit school-level filtering applied.
    /// This guarantees tenant isolation regardless of EF Core global filter behaviour.
    /// </summary>
    private IQueryable<T> SchoolScoped()
    {
        IQueryable<T> query = _dbSet;

        var schoolId = _tenantProvider.GetCurrentSchoolId();
        if (schoolId.HasValue && typeof(BaseEntity).IsAssignableFrom(typeof(T)))
        {
            var id = schoolId.Value;
            query = query.Where(e => EF.Property<int>(e, "SchoolId") == id);
        }

        return query;
    }

    public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);

    public async Task<IEnumerable<T>> GetAllAsync() => await SchoolScoped().ToListAsync();

    public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        => await SchoolScoped().Where(predicate).ToListAsync();

    public IQueryable<T> Query() => SchoolScoped();

    public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);

    public void Update(T entity) => _dbSet.Update(entity);

    public void Remove(T entity) => _dbSet.Remove(entity);

    public async Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null)
        => predicate == null ? await SchoolScoped().CountAsync() : await SchoolScoped().CountAsync(predicate);

    public async Task<bool> AnyAsync(Expression<Func<T, bool>> predicate)
        => await SchoolScoped().AnyAsync(predicate);
}
