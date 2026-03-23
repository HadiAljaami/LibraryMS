using LibraryMS.Application.DTOs.Fines;
using LibraryMS.Application.Services;
using LibraryMS.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryMS.Web.Controllers;

[Authorize]
public class FinesController(
    FineService fineService,
    AppDbContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        var members = await db.Members
            .Where(m => m.TotalFines > m.PaidFines && !m.IsDeleted)
            .OrderByDescending(m => m.TotalFines - m.PaidFines)
            .ToListAsync();

        return View(members);
    }

    [HttpGet]
    public async Task<IActionResult> MemberFines(int memberId)
    {
        var member = await db.Members.FindAsync(memberId);
        if (member is null) return NotFound();

        var result = await fineService.GetByMemberAsync(memberId);
        ViewBag.Member = member;

        return View(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Pay(FinePaymentCreateDto dto)
    {
        var result = await fineService.PayFineAsync(
            dto, User.Identity!.Name!);

        TempData[result.IsSuccess ? "Success" : "Error"] =
            result.IsSuccess
                ? $"تم تسجيل دفع {dto.Amount} ريال بنجاح"
                : result.Error;

        return RedirectToAction(nameof(MemberFines),
            new { memberId = dto.MemberId });
    }
}