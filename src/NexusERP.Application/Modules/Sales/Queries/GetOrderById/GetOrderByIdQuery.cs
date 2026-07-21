using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Application.Modules.Sales.DTOs;
using NexusERP.Domain.Interfaces.Repositories;

namespace NexusERP.Application.Modules.Sales.Queries.GetOrderById;

public class GetOrderByIdQuery : IRequest<ApiResponse<OrderDto>>
{
    public int Id { get; set; }
}

public class GetOrderByIdQueryHandler : IRequestHandler<GetOrderByIdQuery, ApiResponse<OrderDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetOrderByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<OrderDto>> Handle(GetOrderByIdQuery request, CancellationToken cancellationToken)
    {
        var order = await _unitOfWork.Orders.GetByIdWithDetailsAsync(request.Id, cancellationToken);
        if (order is null)
            return ApiResponse<OrderDto>.NotFound("Order not found");

        var dto = new OrderDto
        {
            Id = order.Id,
            OrderNumber = order.OrderNumber,
            OrderType = order.OrderType,
            ClientId = order.ClientId,
            ClientName = order.Client != null ? $"{order.Client.Person.FirstName} {order.Client.Person.LastName}" : null,
            SupplierId = order.SupplierId,
            SupplierName = order.Supplier != null ? $"{order.Supplier.Person.FirstName} {order.Supplier.Person.LastName}" : null,
            OrderDate = order.OrderDate,
            ExpectedDate = order.ExpectedDate,
            Status = order.Status,
            Subtotal = order.Subtotal,
            TaxAmount = order.TaxAmount,
            Total = order.Total,
            Notes = order.Notes,
            ItemCount = order.Details.Count
        };

        return ApiResponse<OrderDto>.Ok(dto);
    }
}
