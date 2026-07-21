using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Domain.Enums;
using NexusERP.Domain.Interfaces.Repositories;
using NexusERP.Domain.Interfaces.Services;

namespace NexusERP.Application.Modules.Inventory.Commands.UpdateProduct;

public class UpdateProductCommand : IRequest<ApiResponse>
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? CategoryId { get; set; }
    public decimal? CostPrice { get; set; }
    public decimal? SellingPrice { get; set; }
    public decimal? TaxRate { get; set; }
    public int? MinStock { get; set; }
    public int? MaxStock { get; set; }
    public string? ImageUrl { get; set; }
    public bool? IsActive { get; set; }
    public bool? IsTaxable { get; set; }
}

public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ApiResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public UpdateProductCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(request.Id, cancellationToken);
        if (product is null)
            return ApiResponse.NotFound("Product not found");

        if (request.Name is not null) product.Name = request.Name;
        if (request.Description is not null) product.Description = request.Description;
        if (request.CategoryId.HasValue) product.CategoryId = request.CategoryId.Value;
        if (request.CostPrice.HasValue) product.CostPrice = request.CostPrice.Value;
        if (request.SellingPrice.HasValue) product.SellingPrice = request.SellingPrice.Value;
        if (request.TaxRate.HasValue) product.TaxRate = request.TaxRate.Value;
        if (request.MinStock.HasValue) product.MinStock = request.MinStock.Value;
        if (request.MaxStock.HasValue) product.MaxStock = request.MaxStock;
        if (request.ImageUrl is not null) product.ImageUrl = request.ImageUrl;
        if (request.IsActive.HasValue) product.IsActive = request.IsActive.Value;
        if (request.IsTaxable.HasValue) product.IsTaxable = request.IsTaxable.Value;

        product.UpdatedBy = _currentUser.UserId;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse.Ok("Product updated successfully");
    }
}
