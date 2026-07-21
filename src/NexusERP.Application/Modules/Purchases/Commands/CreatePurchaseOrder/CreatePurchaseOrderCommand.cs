using FluentValidation;
using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Application.Modules.Purchases.DTOs;
using NexusERP.Domain.Enums;
using NexusERP.Domain.Interfaces.Repositories;
using NexusERP.Domain.Interfaces.Services;

namespace NexusERP.Application.Modules.Purchases.Commands.CreatePurchaseOrder;

public class CreatePurchaseOrderCommand : IRequest<ApiResponse<int>>
{
    public int SupplierId { get; set; }
    public DateTime? ExpectedDate { get; set; }
    public string? Notes { get; set; }
    public List<PurchaseOrderDetailCreateDto> Details { get; set; } = new();
}

public class CreatePurchaseOrderCommandValidator : AbstractValidator<CreatePurchaseOrderCommand>
{
    public CreatePurchaseOrderCommandValidator()
    {
        RuleFor(x => x.SupplierId).GreaterThan(0).WithMessage("Supplier is required");
        RuleFor(x => x.Details).NotEmpty().WithMessage("At least one detail is required");
        RuleForEach(x => x.Details).ChildRules(d =>
        {
            d.RuleFor(x => x.ProductId).GreaterThan(0);
            d.RuleFor(x => x.Quantity).GreaterThan(0);
            d.RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0);
        });
    }
}

public class CreatePurchaseOrderCommandHandler : IRequestHandler<CreatePurchaseOrderCommand, ApiResponse<int>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public CreatePurchaseOrderCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse<int>> Handle(CreatePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        var supplier = await _unitOfWork.Suppliers.GetByIdAsync(request.SupplierId, cancellationToken);
        if (supplier is null)
            return ApiResponse<int>.Fail("Supplier not found");

        var orderNumber = await GenerateOrderNumberAsync(cancellationToken);

        var order = new Domain.Entities.Purchases.PurchaseOrder
        {
            OrderNumber = orderNumber,
            SupplierId = request.SupplierId,
            OrderDate = DateTime.UtcNow,
            ExpectedDate = request.ExpectedDate.HasValue ? DateTime.SpecifyKind(request.ExpectedDate.Value, DateTimeKind.Utc) : null,
            Notes = request.Notes,
            Status = PurchaseOrderStatus.pending,
            CreatedBy = _currentUser.UserId
        };

        decimal subtotal = 0;
        decimal taxAmount = 0;

        foreach (var detail in request.Details)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(detail.ProductId, cancellationToken);
            if (product is null)
                return ApiResponse<int>.Fail($"Product with ID {detail.ProductId} not found");

            var detailSubtotal = detail.Quantity * detail.UnitPrice;
            var detailTax = detailSubtotal * detail.TaxRate / 100;

            order.Details.Add(new Domain.Entities.Purchases.PurchaseOrderDetail
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

        await _unitOfWork.PurchaseOrders.AddAsync(order, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<int>.Ok(order.Id, "Purchase order created successfully");
    }

    private async Task<string> GenerateOrderNumberAsync(CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.ToString("yyyyMMdd");
        var count = await _unitOfWork.PurchaseOrders.CountAsync(o => o.OrderNumber.StartsWith($"OC-{today}"), cancellationToken);
        return $"OC-{today}-{(count + 1):D4}";
    }
}
