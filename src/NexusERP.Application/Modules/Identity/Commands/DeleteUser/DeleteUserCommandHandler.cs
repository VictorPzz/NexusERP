using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Domain.Interfaces.Repositories;

namespace NexusERP.Application.Modules.Identity.Commands.DeleteUser;

public class DeleteUserCommand : IRequest<ApiResponse>
{
    public int Id { get; set; }
}

public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, ApiResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteUserCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.Id, cancellationToken);
        if (user is null)
            return ApiResponse.NotFound("User not found");

        user.IsDeleted = true;
        user.IsActive = false;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse.Ok("User deleted successfully");
    }
}
