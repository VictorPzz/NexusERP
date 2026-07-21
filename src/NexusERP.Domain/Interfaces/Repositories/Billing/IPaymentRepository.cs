using NexusERP.Domain.Entities.Billing;

namespace NexusERP.Domain.Interfaces.Repositories.Billing;

public interface IPaymentRepository : IRepository<Payment>
{
    Task<Payment?> GetByNumberAsync(string paymentNumber, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Payment>> GetByInvoiceIdAsync(int invoiceId, CancellationToken cancellationToken = default);
}
