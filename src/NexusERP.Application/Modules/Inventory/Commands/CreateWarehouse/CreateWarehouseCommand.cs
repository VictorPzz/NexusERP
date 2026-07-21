using MediatR;
using NexusERP.Application.Common.Models;
using FluentValidation;

namespace NexusERP.Application.Modules.Inventory.Commands.CreateWarehouse;

public class CreateWarehouseCommand : IRequest<ApiResponse<int>>
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Address { get; set; }
    public int? ManagerId { get; set; }
    public bool IsDefault { get; set; }
}

public class CreateWarehouseCommandValidator : AbstractValidator<CreateWarehouseCommand>
{
    public CreateWarehouseCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
    }
}

public class CreateWarehouseCommandHandler : IRequestHandler<CreateWarehouseCommand, ApiResponse<int>>
{
    private readonly Domain.Interfaces.Repositories.IUnitOfWork _unitOfWork;

    public CreateWarehouseCommandHandler(Domain.Interfaces.Repositories.IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<int>> Handle(CreateWarehouseCommand request, CancellationToken cancellationToken)
    {
        if (await _unitOfWork.Warehouses.CodeExistsAsync(request.Code, cancellationToken))
            return ApiResponse<int>.Fail("Warehouse code already exists");

        var warehouse = new Domain.Entities.Inventory.Warehouse
        {
            Name = request.Name,
            Code = request.Code,
            Address = request.Address,
            ManagerId = request.ManagerId,
            IsDefault = request.IsDefault,
            IsActive = true
        };

        await _unitOfWork.Warehouses.AddAsync(warehouse, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<int>.Ok(warehouse.Id, "Warehouse created successfully");
    }
}
