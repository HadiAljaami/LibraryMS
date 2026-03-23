using LibraryMS.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryMS.Web.Controllers;

[Authorize(Roles = "Admin")]
public class AuditLogsController(AppDbContext db) : Controller
{
    public async Task<IActionResult> Index(
        string? entity, string? user,
        DateTime? fromDate, DateTime? toDate,
        int page = 1)
    {
        var query = db.AuditLogs.AsQueryable();

        if (!string.IsNullOrEmpty(entity))
            query = query.Where(a => a.EntityName.Contains(entity));

        if (!string.IsNullOrEmpty(user))
            query = query.Where(a => a.UserName.Contains(user));

        if (fromDate.HasValue)
            query = query.Where(a => a.Timestamp >= fromDate);

        if (toDate.HasValue)
            query = query.Where(a => a.Timestamp <= toDate);

        var total = await query.CountAsync();
        var logs  = await query
            .OrderByDescending(a => a.Timestamp)
            .Skip((page - 1) * 20)
            .Take(20)
            .ToListAsync();

        ViewBag.Total    = total;
        ViewBag.Page     = page;
        ViewBag.Entity   = entity;
        ViewBag.User     = user;
        ViewBag.FromDate = fromDate;
        ViewBag.ToDate   = toDate;

        return View(logs);
    }
}