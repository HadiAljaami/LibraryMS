using LibraryMS.Application.DTOs.Reservations;
using LibraryMS.Application.Services;
using LibraryMS.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryMS.Web.Controllers;

[Authorize]
public class ReservationsController(
    ReservationService reservationService,
    AppDbContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        var reservations = await db.Reservations
            .Include(r => r.Book)
            .Include(r => r.Member)
            .Where(r => r.Status == Domain.Enums.ReservationStatus.Pending
                     || r.Status == Domain.Enums.ReservationStatus.Ready)
            .OrderBy(r => r.ReservationDate)
            .ToListAsync();

        return View(reservations);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.Books = await db.Books
            .Include(b => b.Category)
            .Where(b => b.AvailableCopies == 0 && !b.IsDeleted)
            .OrderBy(b => b.Title)
            .ToListAsync();

        ViewBag.Members = await db.Members
            .Where(m => m.IsActive && !m.IsDeleted)
            .OrderBy(m => m.FullName)
            .ToListAsync();

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(ReservationCreateDto dto)
    {
        var result = await reservationService.CreateAsync(
            dto, User.Identity!.Name!);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error;
            return RedirectToAction(nameof(Create));
        }

        TempData["Success"] =
            $"تم الحجز بنجاح — موقعك في القائمة: {result.Value.QueuePosition}";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Cancel(int id)
    {
        var result = await reservationService.CancelAsync(
            id, User.Identity!.Name!);

        TempData[result.IsSuccess ? "Success" : "Error"] =
            result.IsSuccess ? "تم إلغاء الحجز بنجاح" : result.Error;

        return RedirectToAction(nameof(Index));
    }
}