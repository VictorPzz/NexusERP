using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Domain.Entities.Security;
using NexusERP.Domain.Interfaces.Repositories;
using NexusERP.Domain.Interfaces.Services;

namespace NexusERP.Application.Modules.Identity.Commands.CreateUser;

public class CreateUserCommandHandler : IRequestHandler<CreateUserCommand, ApiResponse<int>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public CreateUserCommandHandler(IUnitOfWork unitOfWork, IPasswordHasher passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<ApiResponse<int>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        if (await _unitOfWork.Users.UsernameExistsAsync(request.Username, cancellationToken))
            return ApiResponse<int>.Fail("Username already exists");

        if (await _unitOfWork.Users.EmailExistsAsync(request.Email, cancellationToken))
            return ApiResponse<int>.Fail("Email already exists");

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = _passwordHasher.HashPassword(request.Password),
            IsActive = true
        };

        await _unitOfWork.Users.AddAsync(user, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (request.RoleId.HasValue)
        {
            var role = await _unitOfWork.Roles.GetByIdAsync(request.RoleId.Value, cancellationToken);
            if (role is not null)
            {
                user.UserRoles.Add(new UserRole
                {
                    UserId = user.Id,
                    RoleId = request.RoleId.Value,
                    AssignedAt = DateTime.UtcNow
                });
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
        }

        return ApiResponse<int>.Ok(user.Id, "User created successfully");
    }
}
