using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Application.Modules.Inventory.DTOs;
using NexusERP.Domain.Interfaces.Repositories;

namespace NexusERP.Application.Modules.Inventory.Queries.GetProductById;

public class GetProductByIdQuery : IRequest<ApiResponse<ProductDto>>
{
    public int Id { get; set; }
}

public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ApiResponse<ProductDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetProductByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Products.GetByIdWithCategoryAsync(request.Id, cancellationToken);
        if (product is null)
            return ApiResponse<ProductDto>.NotFound("Product not found");

        var dto = new ProductDto
        {
            Id = product.Id,
            Code = product.Code,
            Name = product.Name,
            Description = product.Description,
            CategoryId = product.CategoryId,
            CategoryName = product.Category.Name,
            UnitOfMeasure = product.UnitOfMeasure,
            CostPrice = product.CostPrice,
            SellingPrice = product.SellingPrice,
            TaxRate = product.TaxRate,
            MinStock = product.MinStock,
            MaxStock = product.MaxStock,
            ImageUrl = product.ImageUrl,
            Barcode = product.Barcode,
            IsActive = product.IsActive,
            IsTaxable = product.IsTaxable
        };

        return ApiResponse<ProductDto>.Ok(dto);
    }
}
