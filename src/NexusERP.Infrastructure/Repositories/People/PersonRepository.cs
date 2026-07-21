using Microsoft.EntityFrameworkCore;
using NexusERP.Domain.Entities.People;
using NexusERP.Domain.Interfaces.Repositories.People;
using NexusERP.Infrastructure.Persistence;

namespace NexusERP.Infrastructure.Repositories.People;

public class PersonRepository : Repository<Person>, IPersonRepository
{
    public PersonRepository(NexusERPDbContext context) : base(context)
    {
    }

    public async Task<Person?> GetByDocumentAsync(string documentNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.DocumentNumber == documentNumber, cancellationToken);
    }

    public async Task<bool> DocumentExistsAsync(string documentNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(p => p.DocumentNumber == documentNumber, cancellationToken);
    }
}
