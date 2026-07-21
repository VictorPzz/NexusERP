using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Application.Modules.Security.Commands.Login;
using NexusERP.Domain.Interfaces.Repositories;
using NexusERP.Domain.Interfaces.Services;
using NexusERP.Domain.Entities.Security;

namespace NexusERP.Application.Modules.Security.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, ApiResponse<AuthResponse>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;

    public RefreshTokenCommandHandler(IUnitOfWork unitOfWork, ITokenService tokenService)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
    }

    public async Task<ApiResponse<AuthResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var storedToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(request.RefreshToken, cancellationToken);

        if (storedToken is null)
            return ApiResponse<AuthResponse>.Fail("Invalid refresh token");

        if (!storedToken.IsActive)
            return ApiResponse<AuthResponse>.Fail("Refresh token has been revoked or expired");

        var userId = _tokenService.GetUserIdFromAccessToken(storedToken.Token);
        if (userId is null)
            return ApiResponse<AuthResponse>.Fail("Invalid token");

        var user = await _unitOfWork.Users.GetByIdWithRolesAsync(storedToken.UserId, cancellationToken);
        if (user is null || !user.IsActive)
            return ApiResponse<AuthResponse>.Fail("User not found or inactive");

        var roles = user.UserRoles.Select(ur => ur.Role.Name).ToList();

        storedToken.IsRevoked = true;
        storedToken.RevokedAt = DateTime.UtcNow;

        var newAccessToken = _tokenService.GenerateAccessToken(user.Id, user.Username, roles);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        var newRefreshEntity = new NexusERP.Domain.Entities.Security.RefreshToken
        {
            UserId = user.Id,
            Token = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
            ReplacedBy = storedToken.Id
        };

        await _unitOfWork.RefreshTokens.AddAsync(newRefreshEntity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse<AuthResponse>.Ok(new AuthResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            ExpiresAt = DateTime.UtcNow.AddMinutes(15),
            User = new UserInfo
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                Roles = roles
            }
        });
    }
}
