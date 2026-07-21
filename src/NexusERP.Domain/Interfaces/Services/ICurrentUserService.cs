namespace NexusERP.Domain.Interfaces.Services;

public interface ICurrentUserService
{
    int? UserId { get; }
    string? Username { get; }
    bool IsAuthenticated { get; }
}
