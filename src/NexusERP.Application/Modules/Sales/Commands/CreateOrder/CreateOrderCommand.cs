using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Application.Modules.Sales.DTOs;
using NexusERP.Domain.Enums;
using FluentValidation;

namespace NexusERP.Application.Modules.Sales.Commands.CreateOrder;

public class CreateOrderCommand : IRequest<ApiResponse<int>>
{
    public OrderType OrderType { get; set; }
    public int? ClientId { get; set; }
    public int? SupplierId { get; set; }
    public DateTime? ExpectedDate { get; set; }
    public string? Notes { get; set; }
    public List<OrderDetailCreateDto> Details { get; set; } = new();
}

public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.Details).NotEmpty().WithMessage("At least one detail is required");
        RuleFor(x => x.ClientId).NotNull().When(x => x.OrderType == OrderType.sale).WithMessage("Client is required for sale orders");
        RuleFor(x => x.SupplierId).NotNull().When(x => x.OrderType == OrderType.purchase).WithMessage("Supplier is required for purchase orders");
        RuleForEach(x => x.Details).ChildRules(d =>
        {
            d.RuleFor(x => x.ProductId).GreaterThan(0);
            d.RuleFor(x => x.Quantity).GreaterThan(0);
            d.RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0);
        });
    }
}

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, ApiResponse<int>>
{
    private readonly Domain.Interfaces.Repositories.IUnitOfWork _unitOfWork;
    private readonly Domain.Interfaces.Services.ICurrentUserService _currentUser;

    public CreateOrderCommandHandler(Domain.Interfaces.Repositories.IUnitOfWork unitOfWork, Domain.Interfaces.Services.ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse<int>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var orderNumber = await GenerateOrderNumberAsync(request.OrderType, cancellationToken);

        var order = new Domain.Entities.Sales.Order
        {
            OrderNumber = orderNumber,
            OrderType = request.OrderType,
            ClientId = request.ClientId,
            SupplierId = request.SupplierId,
            OrderDate = DateTime.UtcNow,
            ExpectedDate = request.ExpectedDate,
            Notes = request.Notes,
            Status = OrderStatus.pending,
            CreatedBy = _currentUser.UserId
        };

        decimal subtotal = 0;
        decimal taxAmount = 0;

        foreach (var detail in request.Details)
        {
            var detailSubtotal = detail.Quantity * detail.UnitPrice;
            var detailTax = detailSubtotal * detail.TaxRate / 100;

            order.Details.Add(new Domain.Entities.Sales.OrderDetail
            {
                ProductId = detail.ProductId,
                Quantity = detail.Quantity,
                UnitPrice = detail.UnitPrice,
                TaxRate = detail.TaxRate,
                Subtotal = detailSubtotal,
                TaxAmount = detailTax,
                Total = detailSubtotal + detailTax
            });

            subtotal += detailSubtotal;
            taxAmount += detailTax;
        }

        order.Subtotal = subtotal;
        order.TaxAmount = taxAmount;
        order.Total = subtotal + taxAmount;

        await _unitOfWork.Orders.AddAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<int>.Ok(order.Id, "Order created successfully");
    }

    private async Task<string> GenerateOrderNumberAsync(OrderType type, CancellationToken cancellationToken)
    {
        var prefix = type == OrderType.sale ? "ORD-V" : "ORD-C";
        var today = DateTime.UtcNow.ToString("yyyyMMdd");
        var count = await _unitOfWork.Orders.CountAsync(o => o.OrderNumber.StartsWith($"{prefix}{today}"), cancellationToken);
        return $"{prefix}{today}-{(count + 1):D4}";
    }
}
