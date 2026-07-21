using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Application.Modules.Inventory.DTOs;
using NexusERP.Domain.Interfaces.Repositories;

namespace NexusERP.Application.Modules.Inventory.Queries.GetAllWarehouses;

public class GetAllWarehousesQuery : IRequest<ApiResponse<List<WarehouseDto>>>
{
}

public class GetAllWarehousesQueryHandler : IRequestHandler<GetAllWarehousesQuery, ApiResponse<List<WarehouseDto>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllWarehousesQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<List<WarehouseDto>>> Handle(GetAllWarehousesQuery request, CancellationToken cancellationToken)
    {
        var warehouses = await _unitOfWork.Warehouses.GetAllAsync(cancellationToken);
        var dtos = warehouses.Select(w => new WarehouseDto
        {
            Id = w.Id,
            Name = w.Name,
            Code = w.Code,
            Address = w.Address,
            ManagerId = w.ManagerId,
            IsActive = w.IsActive,
            IsDefault = w.IsDefault
        }).ToList();

        return ApiResponse<List<WarehouseDto>>.Ok(dtos);
    }
}
