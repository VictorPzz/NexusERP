using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Application.Modules.Billing.DTOs;
using NexusERP.Domain.Enums;
using NexusERP.Domain.Interfaces.Repositories;

namespace NexusERP.Application.Modules.Billing.Queries.GetAllInvoices;

public class GetAllInvoicesQuery : PaginationRequest, IRequest<ApiResponse<PagedResult<InvoiceDto>>>
{
    public InvoiceType? InvoiceType { get; set; }
    public InvoiceStatus? Status { get; set; }
    public int? ClientId { get; set; }
    public int? SupplierId { get; set; }
}

public class GetAllInvoicesQueryHandler : IRequestHandler<GetAllInvoicesQuery, ApiResponse<PagedResult<InvoiceDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllInvoicesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<PagedResult<InvoiceDto>>> Handle(GetAllInvoicesQuery request, CancellationToken cancellationToken)
    {
        var allInvoices = await _unitOfWork.Invoices.GetAllAsync(cancellationToken);
        var query = allInvoices.AsEnumerable();

        if (request.InvoiceType.HasValue)
            query = query.Where(i => i.InvoiceType == request.InvoiceType.Value);

        if (request.Status.HasValue)
            query = query.Where(i => i.Status == request.Status.Value);

        if (request.ClientId.HasValue)
            query = query.Where(i => i.ClientId == request.ClientId.Value);

        if (request.SupplierId.HasValue)
            query = query.Where(i => i.SupplierId == request.SupplierId.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLower();
            query = query.Where(i => i.InvoiceNumber.ToLower().Contains(search));
        }

        var totalCount = query.Count();
        var items = query
            .OrderByDescending(i => i.InvoiceDate)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(i => new InvoiceDto
            {
                Id = i.Id,
                InvoiceNumber = i.InvoiceNumber,
                InvoiceType = i.InvoiceType,
                SaleId = i.SaleId,
                PurchaseId = i.PurchaseId,
                ClientId = i.ClientId,
                SupplierId = i.SupplierId,
                InvoiceDate = i.InvoiceDate,
                DueDate = i.DueDate,
                Status = i.Status,
                Subtotal = i.Subtotal,
                TaxAmount = i.TaxAmount,
                Total = i.Total,
                AmountPaid = i.AmountPaid,
                BalanceDue = i.BalanceDue,
                Notes = i.Notes,
                ItemCount = 0,
                PaymentCount = 0
            })
            .ToList();

        return ApiResponse<PagedResult<InvoiceDto>>.Ok(new PagedResult<InvoiceDto>(items, totalCount, request.Page, request.PageSize));
    }
}
