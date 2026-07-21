using System.Text.Json;
using NexusERP.Domain.Entities.Security;
using NexusERP.Domain.Interfaces.Repositories;
using NexusERP.Domain.Interfaces.Services;

namespace NexusERP.Infrastructure.Services;

public class AuditService : IAuditService
{
    private readonly IUnitOfWork _unitOfWork;

    public AuditService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task LogAsync(int? userId, string action, string entityName, int? entityId,
        object? oldValues = null, object? newValues = null,
        string? ipAddress = null, string? userAgent = null,
        CancellationToken cancellationToken = default)
    {
        var auditLog = new AuditLog
        {
            UserId = userId,
            Action = action,
            EntityName = entityName,
            EntityId = entityId,
            OldValues = oldValues is not null ? JsonDocument.Parse(JsonSerializer.Serialize(oldValues)) : null,
            NewValues = newValues is not null ? JsonDocument.Parse(JsonSerializer.Serialize(newValues)) : null,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.AuditLogs.AddAsync(auditLog, cancellationToken);
    }
}
