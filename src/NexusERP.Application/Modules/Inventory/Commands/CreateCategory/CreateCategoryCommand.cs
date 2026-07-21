using MediatR;
using NexusERP.Application.Common.Models;
using FluentValidation;

namespace NexusERP.Application.Modules.Inventory.Commands.CreateCategory;

public class CreateCategoryCommand : IRequest<ApiResponse<int>>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ParentId { get; set; }
    public string? ImageUrl { get; set; }
    public int SortOrder { get; set; }
}

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}

public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, ApiResponse<int>>
{
    private readonly Domain.Interfaces.Repositories.IUnitOfWork _unitOfWork;

    public CreateCategoryCommandHandler(Domain.Interfaces.Repositories.IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<int>> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        if (await _unitOfWork.Categories.NameExistsAsync(request.Name, request.ParentId, cancellationToken))
            return ApiResponse<int>.Fail("Category name already exists in this level");

        var category = new Domain.Entities.Inventory.Category
        {
            Name = request.Name,
            Description = request.Description,
            ParentId = request.ParentId,
            ImageUrl = request.ImageUrl,
            SortOrder = request.SortOrder,
            IsActive = true
        };

        await _unitOfWork.Categories.AddAsync(category, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<int>.Ok(category.Id, "Category created successfully");
    }
}
