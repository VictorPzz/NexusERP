using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Application.Modules.Billing.DTOs;
using NexusERP.Domain.Enums;
using NexusERP.Domain.Interfaces.Repositories;

namespace NexusERP.Application.Modules.Billing.Queries.GetAllPayments;

public class GetAllPaymentsQuery : PaginationRequest, IRequest<ApiResponse<PagedResult<PaymentDto>>>
{
    public int? InvoiceId { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }
}

public class GetAllPaymentsQueryHandler : IRequestHandler<GetAllPaymentsQuery, ApiResponse<PagedResult<PaymentDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllPaymentsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<PagedResult<PaymentDto>>> Handle(GetAllPaymentsQuery request, CancellationToken cancellationToken)
    {
        var allPayments = await _unitOfWork.Payments.GetAllAsync(cancellationToken);
        var query = allPayments.AsEnumerable();

        if (request.InvoiceId.HasValue)
            query = query.Where(p => p.InvoiceId == request.InvoiceId.Value);

        if (request.PaymentMethod.HasValue)
            query = query.Where(p => p.PaymentMethod == request.PaymentMethod.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLower();
            query = query.Where(p => p.PaymentNumber.ToLower().Contains(search) ||
                                     (p.Reference != null && p.Reference.ToLower().Contains(search)));
        }

        var totalCount = query.Count();
        var items = query
            .OrderByDescending(p => p.PaymentDate)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new PaymentDto
            {
                Id = p.Id,
                PaymentNumber = p.PaymentNumber,
                InvoiceId = p.InvoiceId,
                Amount = p.Amount,
                PaymentMethod = p.PaymentMethod,
                PaymentDate = p.PaymentDate,
                Reference = p.Reference,
                Notes = p.Notes,
                CreatedBy = p.CreatedBy
            })
            .ToList();

        return ApiResponse<PagedResult<PaymentDto>>.Ok(new PagedResult<PaymentDto>(items, totalCount, request.Page, request.PageSize));
    }
}
