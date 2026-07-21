using Microsoft.EntityFrameworkCore;
using NexusERP.Domain.Entities.Billing;
using NexusERP.Domain.Interfaces.Repositories.Billing;
using NexusERP.Infrastructure.Persistence;

namespace NexusERP.Infrastructure.Repositories.Billing;

public class InvoiceRepository : Repository<Invoice>, IInvoiceRepository
{
    public InvoiceRepository(NexusERPDbContext context) : base(context)
    {
    }

    public async Task<Invoice?> GetByNumberAsync(string invoiceNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(i => i.InvoiceNumber == invoiceNumber, cancellationToken);
    }

    public async Task<Invoice?> GetByIdWithDetailsAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(i => i.Client)
                .ThenInclude(c => c.Person)
            .Include(i => i.Supplier)
                .ThenInclude(s => s.Person)
            .Include(i => i.Details)
                .ThenInclude(d => d.Product)
            .Include(i => i.Payments)
            .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
    }
}
