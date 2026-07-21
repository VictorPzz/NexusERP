using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Application.Modules.Purchases.DTOs;
using NexusERP.Domain.Interfaces.Repositories;

namespace NexusERP.Application.Modules.Purchases.Queries.GetPurchaseOrderById;

public class GetPurchaseOrderByIdQuery : IRequest<ApiResponse<PurchaseOrderDto>>
{
    public int Id { get; set; }
}

public class GetPurchaseOrderByIdQueryHandler : IRequestHandler<GetPurchaseOrderByIdQuery, ApiResponse<PurchaseOrderDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetPurchaseOrderByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<PurchaseOrderDto>> Handle(GetPurchaseOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.PurchaseOrders.GetByIdWithDetailsAsync(request.Id, cancellationToken);
        if (order is null)
            return ApiResponse<PurchaseOrderDto>.NotFound("Purchase order not found");

        var dto = new PurchaseOrderDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            SupplierId = order.SupplierId,
            SupplierName = $"{order.Supplier.Person.FirstName} {order.Supplier.Person.LastName}",
            OrderDate = order.OrderDate,
            ExpectedDate = order.ExpectedDate,
            Status = order.Status,
            Subtotal = order.Subtotal,
            TaxAmount = order.TaxAmount,
            Total = order.Total,
            Notes = order.Notes,
            ApprovedBy = order.ApprovedBy,
            ApprovedAt = order.ApprovedAt,
            ItemCount = order.Details.Count
        };

        return ApiResponse<PurchaseOrderDto>.Ok(dto);
    }
}
