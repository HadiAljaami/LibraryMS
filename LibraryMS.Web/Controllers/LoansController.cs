using LibraryMS.Application.DTOs.Loans;
using LibraryMS.Application.Services;
using LibraryMS.Domain.Enums;
using LibraryMS.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryMS.Web.Controllers;

[Authorize]
public class LoansController(
    LoanService loanService,
    AppDbContext db) : Controller
{
    public async Task<IActionResult> Index(
        int? memberId, int? bookId,
        LoanStatus? status, DateTime? fromDate,
        DateTime? toDate, int page = 1)
    {
        var search = new LoanSearchDto(
            memberId, bookId, status ?? LoanStatus.Active,
            fromDate, toDate, page, 12);

        var result = await loanService.SearchAsync(search);
        ViewBag.Search = search;

        return View(result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> Borrow()
    {
        ViewBag.Books = await db.Books
            .Include(b => b.Category)
            .Where(b => b.AvailableCopies > 0 && !b.IsDeleted)
            .OrderBy(b => b.Title)
            .ToListAsync();

        ViewBag.Members = await db.Members
            .Where(m => m.IsActive && !m.IsDeleted)
            .OrderBy(m => m.FullName)
            .ToListAsync();

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Borrow(LoanCreateDto dto)
    {
        var result = await loanService.BorrowAsync(
            dto, User.Identity!.Name!);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error;
            return RedirectToAction(nameof(Borrow));
        }

        TempData["Success"] =
            $"تمت إعارة \"{result.Value.BookTitle}\" بنجاح";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Return(int id)
    {
        var result = await loanService.ReturnAsync(
            id, User.Identity!.Name!);

        TempData[result.IsSuccess ? "Success" : "Error"] =
            result.IsSuccess
                ? $"تم إرجاع \"{result.Value.BookTitle}\" بنجاح"
                : result.Error;

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Renew(LoanRenewDto dto)
    {
        var result = await loanService.RenewAsync(
            dto, User.Identity!.Name!);

        TempData[result.IsSuccess ? "Success" : "Error"] =
            result.IsSuccess
                ? "تم تجديد الاستعارة بنجاح"
                : result.Error;

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Overdue()
    {
        var result = await loanService.GetOverdueAsync();
        return View(result.Value);
    }
}