using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Application.Modules.Sales.DTOs;
using NexusERP.Domain.Enums;
using FluentValidation;

namespace NexusERP.Application.Modules.Sales.Commands.CreateSale;

public class CreateSaleCommand : IRequest<ApiResponse<int>>
{
    public int ClientId { get; set; }
    public string? Notes { get; set; }
    public decimal DiscountAmount { get; set; }
    public List<SaleDetailCreateDto> Details { get; set; } = new();
}

public class CreateSaleCommandValidator : AbstractValidator<CreateSaleCommand>
{
    public CreateSaleCommandValidator()
    {
        RuleFor(x => x.ClientId).GreaterThan(0).WithMessage("Client is required");
        RuleFor(x => x.Details).NotEmpty().WithMessage("At least one detail is required");
        RuleFor(x => x.DiscountAmount).GreaterThanOrEqualTo(0);
        RuleForEach(x => x.Details).ChildRules(d =>
        {
            d.RuleFor(x => x.ProductId).GreaterThan(0);
            d.RuleFor(x => x.Quantity).GreaterThan(0);
            d.RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0);
        });
    }
}

public class CreateSaleCommandHandler : IRequestHandler<CreateSaleCommand, ApiResponse<int>>
{
    private readonly Domain.Interfaces.Repositories.IUnitOfWork _unitOfWork;
    private readonly Domain.Interfaces.Services.ICurrentUserService _currentUser;

    public CreateSaleCommandHandler(Domain.Interfaces.Repositories.IUnitOfWork unitOfWork, Domain.Interfaces.Services.ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse<int>> Handle(CreateSaleCommand request, CancellationToken cancellationToken)
    {
        var client = await _unitOfWork.Clients.GetByIdAsync(request.ClientId, cancellationToken);
        if (client is null)
            return ApiResponse<int>.Fail("Client not found");

        var saleNumber = await GenerateSaleNumberAsync(cancellationToken);

        var sale = new Domain.Entities.Sales.Sale
        {
            SaleNumber = saleNumber,
            ClientId = request.ClientId,
            SaleDate = DateTime.UtcNow,
            DiscountAmount = request.DiscountAmount,
            Notes = request.Notes,
            Status = SaleStatus.pending,
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

            sale.Details.Add(new Domain.Entities.Sales.SaleDetail
            {
                ProductId = detail.ProductId,
                Quantity = detail.Quantity,
                UnitPrice = detail.UnitPrice,
                CostPrice = product.CostPrice,
                TaxRate = detail.TaxRate,
                DiscountRate = detail.DiscountRate,
                Subtotal = detailSubtotal,
                TaxAmount = detailTax,
                DiscountAmount = detailSubtotal * detail.DiscountRate / 100,
                Total = detailSubtotal + detailTax - (detailSubtotal * detail.DiscountRate / 100),
                Profit = (detail.UnitPrice - product.CostPrice) * detail.Quantity
            });

            subtotal += detailSubtotal;
            taxAmount += detailTax;
        }

        sale.Subtotal = subtotal;
        sale.TaxAmount = taxAmount;
        sale.Total = subtotal + taxAmount - request.DiscountAmount;

        await _unitOfWork.Sales.AddAsync(sale, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<int>.Ok(sale.Id, "Sale created successfully");
    }

    private async Task<string> GenerateSaleNumberAsync(CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow.ToString("yyyyMMdd");
        var count = await _unitOfWork.Sales.CountAsync(s => s.SaleNumber.StartsWith($"FAC-{today}"), cancellationToken);
        return $"FAC-{today}-{(count + 1):D4}";
    }
}
