using FluentValidation;
using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Application.Modules.Purchases.DTOs;
using NexusERP.Domain.Enums;
using NexusERP.Domain.Interfaces.Repositories;
using NexusERP.Domain.Interfaces.Services;

namespace NexusERP.Application.Modules.Purchases.Commands.CreatePurchase;

public class CreatePurchaseCommand : IRequest<ApiResponse<int>>
{
    public int SupplierId { get; set; }
    public int? PurchaseOrderId { get; set; }
    public string? Notes { get; set; }
    public List<PurchaseDetailCreateDto> Details { get; set; } = new();
}

public class CreatePurchaseCommandValidator : AbstractValidator<CreatePurchaseCommand>
{
    public CreatePurchaseCommandValidator()
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

public class CreatePurchaseCommandHandler : IRequestHandler<CreatePurchaseCommand, ApiResponse<int>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public CreatePurchaseCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse<int>> Handle(CreatePurchaseCommand request, CancellationToken cancellationToken)
    {
        var supplier = await _unitOfWork.Suppliers.GetByIdAsync(request.SupplierId, cancellationToken);
        if (supplier is null)
            return ApiResponse<int>.Fail("Supplier not found");

        if (request.PurchaseOrderId.HasValue)
        {
            var purchaseOrder = await _unitOfWork.PurchaseOrders.GetByIdAsync(request.PurchaseOrderId.Value, cancellationToken);
            if (purchaseOrder is null)
                return ApiResponse<int>.Fail("Purchase order not found");
        }

        var purchaseNumber = await GeneratePurchaseNumberAsync(cancellationToken);

        var purchase = new Domain.Entities.Purchases.Purchase
        {
            PurchaseNumber = purchaseNumber,
            SupplierId = request.SupplierId,
            PurchaseOrderId = request.PurchaseOrderId,
            PurchaseDate = DateTime.UtcNow,
            Notes = request.Notes,
            Status = PurchaseStatus.pending,
            PaymentStatus = PaymentStatus.pending,
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

            purchase.Details.Add(new Domain.Entities.Purchases.PurchaseDetail
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

        purchase.Subtotal = subtotal;
        purchase.TaxAmount = taxAmount;
        purchase.Total = subtotal + taxAmount;

        await _unitOfWork.Purchases.AddAsync(purchase, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<int>.Ok(purchase.Id, "Purchase created successfully");
    }

    private async Task<string> GeneratePurchaseNumberAsync(CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.ToString("yyyyMMdd");
        var count = await _unitOfWork.Purchases.CountAsync(p => p.PurchaseNumber.StartsWith($"CMP-{today}"), cancellationToken);
        return $"CMP-{today}-{(count + 1):D4}";
    }
}
