using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Domain.Entities.Security;
using NexusERP.Domain.Interfaces.Repositories;

namespace NexusERP.Application.Modules.Identity.Commands.CreateRole;

public class CreateRoleCommandHandler : IRequestHandler<CreateRoleCommand, ApiResponse<int>>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateRoleCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<int>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        if (await _unitOfWork.Roles.NameExistsAsync(request.Name, cancellationToken))
            return ApiResponse<int>.Fail("Role name already exists");

        var role = new Role
        {
            Name = request.Name,
            Description = request.Description,
            IsActive = true
        };

        await _unitOfWork.Roles.AddAsync(role, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<int>.Ok(role.Id, "Role created successfully");
    }
}
