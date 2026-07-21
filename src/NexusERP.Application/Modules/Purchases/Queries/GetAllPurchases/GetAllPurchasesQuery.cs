using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Application.Modules.Purchases.DTOs;
using NexusERP.Domain.Enums;
using NexusERP.Domain.Interfaces.Repositories;

namespace NexusERP.Application.Modules.Purchases.Queries.GetAllPurchases;

public class GetAllPurchasesQuery : PaginationRequest, IRequest<ApiResponse<PagedResult<PurchaseDto>>>
{
    public PurchaseStatus? Status { get; set; }
    public PaymentStatus? PaymentStatus { get; set; }
    public int? SupplierId { get; set; }
}

public class GetAllPurchasesQueryHandler : IRequestHandler<GetAllPurchasesQuery, ApiResponse<PagedResult<PurchaseDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllPurchasesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<PagedResult<PurchaseDto>>> Handle(GetAllPurchasesQuery request, CancellationToken cancellationToken)
    {
        var allPurchases = await _unitOfWork.Purchases.GetAllAsync(cancellationToken);
        var query = allPurchases.AsEnumerable();

        if (request.Status.HasValue)
            query = query.Where(p => p.Status == request.Status.Value);

        if (request.PaymentStatus.HasValue)
            query = query.Where(p => p.PaymentStatus == request.PaymentStatus.Value);

        if (request.SupplierId.HasValue)
            query = query.Where(p => p.SupplierId == request.SupplierId.Value);

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLower();
            query = query.Where(p => p.PurchaseNumber.ToLower().Contains(search));
        }

        var totalCount = query.Count();
        var items = query
            .OrderByDescending(p => p.PurchaseDate)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new PurchaseDto
            {
                Id = p.Id,
                PurchaseNumber = p.PurchaseNumber,
                SupplierId = p.SupplierId,
                PurchaseOrderId = p.PurchaseOrderId,
                PurchaseDate = p.PurchaseDate,
                Status = p.Status,
                PaymentStatus = p.PaymentStatus,
                Subtotal = p.Subtotal,
                TaxAmount = p.TaxAmount,
                Total = p.Total,
                Notes = p.Notes,
                ItemCount = 0
            })
            .ToList();

        return ApiResponse<PagedResult<PurchaseDto>>.Ok(new PagedResult<PurchaseDto>(items, totalCount, request.Page, request.PageSize));
    }
}
