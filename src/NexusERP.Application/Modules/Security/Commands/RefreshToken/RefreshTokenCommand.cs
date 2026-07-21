using MediatR;
using NexusERP.Application.Common.Models;
using NexusERP.Application.Modules.Security.Commands.Login;

namespace NexusERP.Application.Modules.Security.Commands.RefreshToken;

public class RefreshTokenCommand : IRequest<ApiResponse<AuthResponse>>
{
    public string RefreshToken { get; set; } = string.Empty;
}
