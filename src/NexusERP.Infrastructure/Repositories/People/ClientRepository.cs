using Microsoft.EntityFrameworkCore;
using NexusERP.Domain.Entities.People;
using NexusERP.Domain.Interfaces.Repositories.People;
using NexusERP.Infrastructure.Persistence;

namespace NexusERP.Infrastructure.Repositories.People;

public class ClientRepository : Repository<Client>, IClientRepository
{
    public ClientRepository(NexusERPDbContext context) : base(context)
    {
    }

    public async Task<Client?> GetByCodeAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.ClientCode == code, cancellationToken);
    }

    public async Task<Client?> GetByIdWithPersonAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Person)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Client?> GetByIdWithAddressesAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Addresses)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<bool> CodeExistsAsync(string code, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(c => c.ClientCode == code, cancellationToken);
    }

    public async Task<List<Client>> GetAllWithPersonAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Person)
            .Include(c => c.Addresses)
            .ToListAsync(cancellationToken);
    }
}
