namespace NexusERP.Domain.Interfaces.Services;

public interface IAuditService
{
    Task LogAsync(int? userId, string action, string entityName, int? entityId,
        object? oldValues = null, object? newValues = null,
        string? ipAddress = null, string? userAgent = null,
        CancellationToken cancellationToken = default);
}
