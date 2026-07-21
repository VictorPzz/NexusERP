using Microsoft.EntityFrameworkCore;
using NexusERP.Domain.Entities.Billing;
using NexusERP.Domain.Interfaces.Repositories.Billing;
using NexusERP.Infrastructure.Persistence;

namespace NexusERP.Infrastructure.Repositories.Billing;

public class PaymentRepository : Repository<Payment>, IPaymentRepository
{
    public PaymentRepository(NexusERPDbContext context) : base(context)
    {
    }

    public async Task<Payment?> GetByNumberAsync(string paymentNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(p => p.PaymentNumber == paymentNumber, cancellationToken);
    }

    public async Task<IReadOnlyList<Payment>> GetByInvoiceIdAsync(int invoiceId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.InvoiceId == invoiceId)
            .OrderByDescending(p => p.PaymentDate)
            .ToListAsync(cancellationToken);
    }
}
