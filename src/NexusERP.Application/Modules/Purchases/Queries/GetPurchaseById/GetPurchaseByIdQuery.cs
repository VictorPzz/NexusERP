using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Application.Modules.Purchases.DTOs;
using NexusERP.Domain.Interfaces.Repositories;

namespace NexusERP.Application.Modules.Purchases.Queries.GetPurchaseById;

public class GetPurchaseByIdQuery : IRequest<ApiResponse<PurchaseDto>>
{
    public int Id { get; set; }
}

public class GetPurchaseByIdQueryHandler : IRequestHandler<GetPurchaseByIdQuery, ApiResponse<PurchaseDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPurchaseByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<PurchaseDto>> Handle(GetPurchaseByIdQuery request, CancellationToken cancellationToken)
    {
        var purchase = await _unitOfWork.Purchases.GetByIdWithDetailsAsync(request.Id, cancellationToken);
        if (purchase is null)
            return ApiResponse<PurchaseDto>.NotFound("Purchase not found");

        var dto = new PurchaseDto
        {
            Id = purchase.Id,
            PurchaseNumber = purchase.PurchaseNumber,
            SupplierId = purchase.SupplierId,
            SupplierName = $"{purchase.Supplier.Person.FirstName} {purchase.Supplier.Person.LastName}",
            PurchaseOrderId = purchase.PurchaseOrderId,
            PurchaseDate = purchase.PurchaseDate,
            Status = purchase.Status,
            PaymentStatus = purchase.PaymentStatus,
            Subtotal = purchase.Subtotal,
            TaxAmount = purchase.TaxAmount,
            Total = purchase.Total,
            Notes = purchase.Notes,
            ItemCount = purchase.Details.Count
        };

        return ApiResponse<PurchaseDto>.Ok(dto);
    }
}
