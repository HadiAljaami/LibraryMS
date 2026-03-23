using LibraryMS.Application.DTOs.Members;
using LibraryMS.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryMS.Web.Controllers;

[Authorize]
public class MembersController(MemberService memberService) : Controller
{
    public async Task<IActionResult> Index(
        string? name, string? email,
        string? membershipNumber, bool? isActive,
        int page = 1)
    {
        var search = new MemberSearchDto(
            name, email, membershipNumber, isActive, page, 12);

        var result = await memberService.SearchAsync(search);
        ViewBag.Search = search;

        return View(result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var result = await memberService.GetByIdAsync(id);
        if (result.IsFailure)
            return NotFound();

        return View(result.Value);
    }

    [HttpGet]
    public IActionResult Create() => View();

    [HttpPost]
    public async Task<IActionResult> Create(MemberCreateDto dto)
    {
        var result = await memberService.CreateAsync(dto);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error;
            return View(dto);
        }

        TempData["Success"] =
            $"تمت إضافة العضو \"{dto.FullName}\" بنجاح";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var result = await memberService.GetByIdAsync(id);
        if (result.IsFailure)
            return NotFound();

        var dto = new MemberUpdateDto(
            result.Value.FullName,
            result.Value.Email,
            result.Value.PhoneNumber,
            result.Value.Address,
            null,
            result.Value.MembershipExpiry,
            result.Value.IsActive);

        return View(dto);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, MemberUpdateDto dto)
    {
        var result = await memberService.UpdateAsync(id, dto);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error;
            return View(dto);
        }

        TempData["Success"] = "تم تعديل بيانات العضو بنجاح";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await memberService.DeleteAsync(id);

        TempData[result.IsSuccess ? "Success" : "Error"] =
            result.IsSuccess ? "تم حذف العضو بنجاح" : result.Error;

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> ToggleStatus(int id)
    {
        var result = await memberService.ToggleStatusAsync(id);

        TempData[result.IsSuccess ? "Success" : "Error"] =
            result.IsSuccess
                ? "تم تغيير حالة العضو بنجاح"
                : result.Error;

        return RedirectToAction(nameof(Index));
    }
}