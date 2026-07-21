using FluentValidation;
using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Application.Modules.Billing.DTOs;
using NexusERP.Domain.Enums;
using NexusERP.Domain.Interfaces.Repositories;
using NexusERP.Domain.Interfaces.Services;

namespace NexusERP.Application.Modules.Billing.Commands.CreateInvoice;

public class CreateInvoiceCommand : IRequest<ApiResponse<int>>
{
    public InvoiceType InvoiceType { get; set; }
    public int? SaleId { get; set; }
    public int? PurchaseId { get; set; }
    public int? ClientId { get; set; }
    public int? SupplierId { get; set; }
    public DateTime? DueDate { get; set; }
    public string? Notes { get; set; }
    public List<InvoiceDetailCreateDto> Details { get; set; } = new();
}

public class CreateInvoiceCommandValidator : AbstractValidator<CreateInvoiceCommand>
{
    public CreateInvoiceCommandValidator()
    {
        RuleFor(x => x.Details).NotEmpty().WithMessage("At least one detail is required");
        RuleFor(x => x.ClientId).NotNull().When(x => x.InvoiceType == InvoiceType.sale).WithMessage("Client is required for sale invoices");
        RuleFor(x => x.SupplierId).NotNull().When(x => x.InvoiceType == InvoiceType.purchase).WithMessage("Supplier is required for purchase invoices");
        RuleForEach(x => x.Details).ChildRules(d =>
        {
            d.RuleFor(x => x.ProductId).GreaterThan(0);
            d.RuleFor(x => x.Quantity).GreaterThan(0);
            d.RuleFor(x => x.UnitPrice).GreaterThanOrEqualTo(0);
        });
    }
}

public class CreateInvoiceCommandHandler : IRequestHandler<CreateInvoiceCommand, ApiResponse<int>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public CreateInvoiceCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse<int>> Handle(CreateInvoiceCommand request, CancellationToken cancellationToken)
    {
        var invoiceNumber = await GenerateInvoiceNumberAsync(cancellationToken);

        var invoice = new Domain.Entities.Billing.Invoice
        {
            InvoiceNumber = invoiceNumber,
            InvoiceType = request.InvoiceType,
            SaleId = request.SaleId,
            PurchaseId = request.PurchaseId,
            ClientId = request.ClientId,
            SupplierId = request.SupplierId,
            InvoiceDate = DateTime.UtcNow,
            DueDate = request.DueDate.HasValue ? DateTime.SpecifyKind(request.DueDate.Value, DateTimeKind.Utc) : null,
            Notes = request.Notes,
            Status = InvoiceStatus.issued,
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

            invoice.Details.Add(new Domain.Entities.Billing.InvoiceDetail
            {
                ProductId = detail.ProductId,
                Description = detail.Description ?? product.Name,
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

        invoice.Subtotal = subtotal;
        invoice.TaxAmount = taxAmount;
        invoice.Total = subtotal + taxAmount;
        invoice.BalanceDue = invoice.Total;

        await _unitOfWork.Invoices.AddAsync(invoice, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<int>.Ok(invoice.Id, "Invoice created successfully");
    }

    private async Task<string> GenerateInvoiceNumberAsync(CancellationToken cancellationToken)
    {
        var prefix = "FACB";
        var today = DateTime.UtcNow.ToString("yyyyMMdd");
        var count = await _unitOfWork.Invoices.CountAsync(i => i.InvoiceNumber.StartsWith($"{prefix}-{today}"), cancellationToken);
        return $"{prefix}-{today}-{(count + 1):D4}";
    }
}
