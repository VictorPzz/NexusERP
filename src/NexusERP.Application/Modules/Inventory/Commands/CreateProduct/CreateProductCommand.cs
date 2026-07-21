using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Domain.Enums;
using FluentValidation;

namespace NexusERP.Application.Modules.Inventory.Commands.CreateProduct;

public class CreateProductCommand : IRequest<ApiResponse<int>>
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int CategoryId { get; set; }
    public UnitOfMeasure UnitOfMeasure { get; set; } = UnitOfMeasure.unit;
    public decimal CostPrice { get; set; }
    public decimal SellingPrice { get; set; }
    public decimal TaxRate { get; set; }
    public int MinStock { get; set; }
    public int? MaxStock { get; set; }
    public string? ImageUrl { get; set; }
    public string? Barcode { get; set; }
    public bool IsTaxable { get; set; } = true;
}

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.CategoryId).GreaterThan(0);
        RuleFor(x => x.CostPrice).GreaterThanOrEqualTo(0);
        RuleFor(x => x.SellingPrice).GreaterThan(0);
        RuleFor(x => x.TaxRate).GreaterThanOrEqualTo(0).LessThanOrEqualTo(100);
        RuleFor(x => x.MinStock).GreaterThanOrEqualTo(0);
    }
}

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ApiResponse<int>>
{
    private readonly Domain.Interfaces.Repositories.IUnitOfWork _unitOfWork;
    private readonly Domain.Interfaces.Services.ICurrentUserService _currentUser;

    public CreateProductCommandHandler(Domain.Interfaces.Repositories.IUnitOfWork unitOfWork, Domain.Interfaces.Services.ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse<int>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        if (await _unitOfWork.Products.CodeExistsAsync(request.Code, cancellationToken))
            return ApiResponse<int>.Fail("Product code already exists");

        if (!string.IsNullOrEmpty(request.Barcode) && await _unitOfWork.Products.BarcodeExistsAsync(request.Barcode, cancellationToken))
            return ApiResponse<int>.Fail("Barcode already exists");

        var product = new Domain.Entities.Inventory.Product
        {
            Code = request.Code,
            Name = request.Name,
            Description = request.Description,
            CategoryId = request.CategoryId,
            UnitOfMeasure = request.UnitOfMeasure,
            CostPrice = request.CostPrice,
            SellingPrice = request.SellingPrice,
            TaxRate = request.TaxRate,
            MinStock = request.MinStock,
            MaxStock = request.MaxStock,
            ImageUrl = request.ImageUrl,
            Barcode = request.Barcode,
            IsTaxable = request.IsTaxable,
            IsActive = true,
            CreatedBy = _currentUser.UserId
        };

        await _unitOfWork.Products.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<int>.Ok(product.Id, "Product created successfully");
    }
}
