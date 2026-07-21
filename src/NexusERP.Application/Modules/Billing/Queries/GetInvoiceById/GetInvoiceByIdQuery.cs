using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Application.Modules.Billing.DTOs;
using NexusERP.Domain.Interfaces.Repositories;

namespace NexusERP.Application.Modules.Billing.Queries.GetInvoiceById;

public class GetInvoiceByIdQuery : IRequest<ApiResponse<InvoiceDto>>
{
    public int Id { get; set; }
}

public class GetInvoiceByIdQueryHandler : IRequestHandler<GetInvoiceByIdQuery, ApiResponse<InvoiceDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetInvoiceByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<InvoiceDto>> Handle(GetInvoiceByIdQuery request, CancellationToken cancellationToken)
    {
        var invoice = await _unitOfWork.Invoices.GetByIdWithDetailsAsync(request.Id, cancellationToken);
        if (invoice is null)
            return ApiResponse<InvoiceDto>.NotFound("Invoice not found");

        var dto = new InvoiceDto
        {
            Id = invoice.Id,
            InvoiceNumber = invoice.InvoiceNumber,
            InvoiceType = invoice.InvoiceType,
            SaleId = invoice.SaleId,
            PurchaseId = invoice.PurchaseId,
            ClientId = invoice.ClientId,
            ClientName = invoice.Client != null ? $"{invoice.Client.Person.FirstName} {invoice.Client.Person.LastName}" : null,
            SupplierId = invoice.SupplierId,
            SupplierName = invoice.Supplier != null ? $"{invoice.Supplier.Person.FirstName} {invoice.Supplier.Person.LastName}" : null,
            InvoiceDate = invoice.InvoiceDate,
            DueDate = invoice.DueDate,
            Status = invoice.Status,
            Subtotal = invoice.Subtotal,
            TaxAmount = invoice.TaxAmount,
            Total = invoice.Total,
            AmountPaid = invoice.AmountPaid,
            BalanceDue = invoice.BalanceDue,
            Notes = invoice.Notes,
            ItemCount = invoice.Details.Count,
            PaymentCount = invoice.Payments.Count
        };

        return ApiResponse<InvoiceDto>.Ok(dto);
    }
}
