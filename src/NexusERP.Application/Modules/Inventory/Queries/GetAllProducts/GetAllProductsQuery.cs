using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Application.Modules.Inventory.DTOs;
using NexusERP.Domain.Interfaces.Repositories;

namespace NexusERP.Application.Modules.Inventory.Queries.GetAllProducts;

public class GetAllProductsQuery : PaginationRequest, IRequest<ApiResponse<PagedResult<ProductDto>>>
{
}

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, ApiResponse<PagedResult<ProductDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllProductsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<PagedResult<ProductDto>>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var allProducts = await _unitOfWork.Products.GetAllWithCategoryAsync(cancellationToken);
        var query = allProducts.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var search = request.Search.ToLower();
            query = query.Where(p =>
                p.Name.ToLower().Contains(search) ||
                p.Code.ToLower().Contains(search));
        }

        var totalCount = query.Count();
        var items = query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(p => new ProductDto
            {
                Id = p.Id,
                Code = p.Code,
                Name = p.Name,
                Description = p.Description,
                CategoryId = p.CategoryId,
                CategoryName = p.Category.Name,
                UnitOfMeasure = p.UnitOfMeasure,
                CostPrice = p.CostPrice,
                SellingPrice = p.SellingPrice,
                TaxRate = p.TaxRate,
                MinStock = p.MinStock,
                MaxStock = p.MaxStock,
                ImageUrl = p.ImageUrl,
                Barcode = p.Barcode,
                IsActive = p.IsActive,
                IsTaxable = p.IsTaxable
            })
            .ToList();

        var pagedResult = new PagedResult<ProductDto>(items, totalCount, request.Page, request.PageSize);
        return ApiResponse<PagedResult<ProductDto>>.Ok(pagedResult);
    }
}
