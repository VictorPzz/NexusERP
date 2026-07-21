using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Application.Modules.Inventory.DTOs;
using NexusERP.Domain.Interfaces.Repositories;

namespace NexusERP.Application.Modules.Inventory.Queries.GetAllCategories;

public class GetAllCategoriesQuery : IRequest<ApiResponse<List<CategoryDto>>>
{
}

public class GetAllCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, ApiResponse<List<CategoryDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllCategoriesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<List<CategoryDto>>> Handle(GetAllCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _unitOfWork.Categories.GetAllAsync(cancellationToken);
        var dtos = categories.Select(c => new CategoryDto
        {
            Id = c.Id,
            Name = c.Name,
            Description = c.Description,
            ParentId = c.ParentId,
            ParentName = c.Parent?.Name,
            ImageUrl = c.ImageUrl,
            SortOrder = c.SortOrder,
            IsActive = c.IsActive,
            ProductCount = c.Products.Count,
            ChildrenCount = c.Children.Count
        }).ToList();

        return ApiResponse<List<CategoryDto>>.Ok(dtos);
    }
}
