using System.Text.Json;
using LibraryMS.Domain.Entities;
using LibraryMS.Domain.Enums;
using LibraryMS.Infrastructure.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace LibraryMS.Infrastructure.Audit;

public class AuditService(AppDbContext db, IHttpContextAccessor httpContext)
{
    private readonly string _userId = httpContext.HttpContext?.User?
        .FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "system";

    private readonly string _userName = httpContext.HttpContext?.User?
        .Identity?.Name ?? "system";

    private readonly string _ipAddress = httpContext.HttpContext?
        .Connection?.RemoteIpAddress?.ToString() ?? "unknown";

    public async Task LogAsync(
        string entityName,
        string entityId,
        AuditAction action,
        object? oldValues = null,
        object? newValues = null)
    {
        var log = new AuditLog
        {
            EntityName = entityName,
            EntityId   = entityId,
            Action     = action,
            OldValues  = oldValues is null
                ? null
                : JsonSerializer.Serialize(oldValues),
            NewValues  = newValues is null
                ? null
                : JsonSerializer.Serialize(newValues),
            UserId    = _userId,
            UserName  = _userName,
            IpAddress = _ipAddress,
            Timestamp = DateTime.UtcNow
        };

        db.AuditLogs.Add(log);
        await db.SaveChangesAsync();
    }
}