using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Domain.Interfaces.Repositories;
using NexusERP.Domain.Interfaces.Services;

namespace NexusERP.Application.Modules.Inventory.Commands.DeleteProduct;

public class DeleteProductCommand : IRequest<ApiResponse>
{
    public int Id { get; set; }
}

public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, ApiResponse>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICurrentUserService _currentUser;

    public DeleteProductCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUser)
    {
        _unitOfWork = unitOfWork;
        _currentUser = currentUser;
    }

    public async Task<ApiResponse> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(request.Id, cancellationToken);
        if (product is null)
            return ApiResponse.NotFound("Product not found");

        product.IsDeleted = true;
        product.IsActive = false;
        product.UpdatedBy = _currentUser.UserId;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse.Ok("Product deleted successfully");
    }
}
