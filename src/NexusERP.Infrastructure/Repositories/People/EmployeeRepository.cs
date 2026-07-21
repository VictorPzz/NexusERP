using Microsoft.EntityFrameworkCore;
using NexusERP.Domain.Entities.People;
using NexusERP.Domain.Interfaces.Repositories.People;
using NexusERP.Infrastructure.Persistence;

namespace NexusERP.Infrastructure.Repositories.People;

public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(NexusERPDbContext context) : base(context)
    {
    }

    public async Task<Employee?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(e => e.EmployeeCode == code, cancellationToken);
    }

    public async Task<Employee?> GetByIdWithPersonAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(e => e.Person)
            .Include(e => e.JobPosition)
                .ThenInclude(jp => jp.Department)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }

    public async Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(e => e.EmployeeCode == code, cancellationToken);
    }

    public async Task<List<Employee>> GetAllWithPersonAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(e => e.Person)
            .Include(e => e.JobPosition)
                .ThenInclude(jp => jp.Department)
            .ToListAsync(cancellationToken);
    }
}
