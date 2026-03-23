using LibraryMS.Application.Services;
using LibraryMS.Domain.Enums;
using LibraryMS.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryMS.Web.Controllers;

[Authorize]
public class HomeController(AppDbContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        ViewBag.TotalBooks    = await db.Books.CountAsync();
        ViewBag.TotalMembers  = await db.Members.CountAsync();
        ViewBag.ActiveLoans   = await db.Loans
            .CountAsync(l => l.Status == LoanStatus.Active);
        ViewBag.OverdueLoans  = await db.Loans
            .CountAsync(l => l.Status == LoanStatus.Overdue);
        ViewBag.TotalCategories = await db.Categories.CountAsync();
        ViewBag.PendingReservations = await db.Reservations
            .CountAsync(r => r.Status == ReservationStatus.Pending);
        ViewBag.UnpaidFines = await db.Members
            .SumAsync(m => m.TotalFines - m.PaidFines);
        ViewBag.NewMembersThisMonth = await db.Members
            .CountAsync(m => m.JoinDate.Month == DateTime.Now.Month
                          && m.JoinDate.Year  == DateTime.Now.Year);

        var recentLoans = await db.Loans
            .Include(l => l.Book)
            .Include(l => l.Member)
            .OrderByDescending(l => l.BorrowDate)
            .Take(8)
            .ToListAsync();

        var monthlyLoans = await db.Loans
            .Where(l => l.BorrowDate >= DateTime.Now.AddMonths(-6))
            .GroupBy(l => new {
                l.BorrowDate.Year,
                l.BorrowDate.Month
            })
            .Select(g => new {
                g.Key.Year,
                g.Key.Month,
                Count = g.Count()
            })
            .OrderBy(x => x.Year).ThenBy(x => x.Month)
            .ToListAsync();

        ViewBag.MonthlyLoans  = monthlyLoans;
        ViewBag.RecentLoans   = recentLoans;

        return View();
    }

    public IActionResult Error() => View();
}