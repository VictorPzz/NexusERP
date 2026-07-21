using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Domain.Interfaces.Repositories;

namespace NexusERP.Application.Modules.Identity.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, ApiResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateUserCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.Id, cancellationToken);
        if (user is null)
            return ApiResponse.NotFound("User not found");

        if (request.Email is not null && request.Email != user.Email)
        {
            if (await _unitOfWork.Users.EmailExistsAsync(request.Email, cancellationToken))
                return ApiResponse.Fail("Email already exists");
            user.Email = request.Email;
        }

        if (request.IsActive.HasValue)
            user.IsActive = request.IsActive.Value;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return ApiResponse.Ok("User updated successfully");
    }
}
