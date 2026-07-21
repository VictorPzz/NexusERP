using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Domain.Interfaces.Repositories;
using NexusERP.Domain.Interfaces.Services;

namespace NexusERP.Application.Modules.Security.Commands.RevokeToken;

public class RevokeTokenCommand : IRequest<ApiResponse>
{
    public string RefreshToken { get; set; } = string.Empty;
}

public class RevokeTokenCommandHandler : IRequestHandler<RevokeTokenCommand, ApiResponse>
{
    private readonly IUnitOfWork _unitOfWork;

    public RevokeTokenCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse> Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
    {
        var storedToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(request.RefreshToken, cancellationToken);

        if (storedToken is null)
            return ApiResponse.Fail("Invalid refresh token");

        if (storedToken.IsRevoked)
            return ApiResponse.Fail("Token already revoked");

        storedToken.IsRevoked = true;
        storedToken.RevokedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return ApiResponse.Ok("Token revoked successfully");
    }
}
