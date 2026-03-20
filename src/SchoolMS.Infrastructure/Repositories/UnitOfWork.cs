using SchoolMS.Domain.Interfaces;
using SchoolMS.Infrastructure.Data;

namespace SchoolMS.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly SchoolDbContext _context;

    public UnitOfWork(SchoolDbContext context)
    {
        _context = context;
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);

    public void Dispose() => _context.Dispose();
}
