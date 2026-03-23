using LibraryMS.Application.DTOs.Categories;
using LibraryMS.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryMS.Web.Controllers;

[Authorize(Roles = "Admin")]
public class CategoriesController(CategoryService categoryService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var result = await categoryService.GetAllAsync();
        return View(result.Value);
    }

    [HttpGet]
    public IActionResult Create() => View();

    [HttpPost]
    public async Task<IActionResult> Create(CategoryCreateDto dto)
    {
        var result = await categoryService.CreateAsync(dto);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error;
            return View(dto);
        }

        TempData["Success"] =
            $"تمت إضافة تصنيف \"{dto.Name}\" بنجاح";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var result = await categoryService.GetByIdAsync(id);
        if (result.IsFailure)
            return NotFound();

        return View(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, CategoryUpdateDto dto)
    {
        var result = await categoryService.UpdateAsync(id, dto);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error;
            return View(dto);
        }

        TempData["Success"] = "تم تعديل التصنيف بنجاح";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await categoryService.DeleteAsync(id);

        TempData[result.IsSuccess ? "Success" : "Error"] =
            result.IsSuccess ? "تم حذف التصنيف بنجاح" : result.Error;

        return RedirectToAction(nameof(Index));
    }
}