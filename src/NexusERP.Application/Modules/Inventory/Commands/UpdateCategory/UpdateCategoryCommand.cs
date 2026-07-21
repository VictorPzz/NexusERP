using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Domain.Interfaces.Repositories;

namespace NexusERP.Application.Modules.Inventory.Commands.UpdateCategory;

public class UpdateCategoryCommand : IRequest<ApiResponse>
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public int? SortOrder { get; set; }
    public bool? IsActive { get; set; }
}

public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, ApiResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateCategoryCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(request.Id, cancellationToken);
        if (category is null)
            return ApiResponse.NotFound("Category not found");

        if (request.Name is not null) category.Name = request.Name;
        if (request.Description is not null) category.Description = request.Description;
        if (request.ImageUrl is not null) category.ImageUrl = request.ImageUrl;
        if (request.SortOrder.HasValue) category.SortOrder = request.SortOrder.Value;
        if (request.IsActive.HasValue) category.IsActive = request.IsActive.Value;

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse.Ok("Category updated successfully");
    }
}
