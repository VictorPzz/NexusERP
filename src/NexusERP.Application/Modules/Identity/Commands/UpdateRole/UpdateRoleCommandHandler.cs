using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Domain.Interfaces.Repositories;

namespace NexusERP.Application.Modules.Identity.Commands.UpdateRole;

public class UpdateRoleCommandHandler : IRequestHandler<UpdateRoleCommand, ApiResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateRoleCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await _unitOfWork.Roles.GetByIdAsync(request.Id, cancellationToken);
        if (role is null)
            return ApiResponse.NotFound("Role not found");

        if (request.Name is not null && request.Name != role.Name)
        {
            if (await _unitOfWork.Roles.NameExistsAsync(request.Name, cancellationToken))
                return ApiResponse.Fail("Role name already exists");
            role.Name = request.Name;
        }

        if (request.Description is not null)
            role.Description = request.Description;

        if (request.IsActive.HasValue)
            role.IsActive = request.IsActive.Value;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ApiResponse.Ok("Role updated successfully");
    }
}
