using LibraryMS.Application.DTOs.Books;
using LibraryMS.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryMS.Web.Controllers;

[Authorize]
public class BooksController(
    BookService bookService,
    CategoryService categoryService) : Controller
{
    public async Task<IActionResult> Index(
        string? title, string? author, string? isbn,
        int? categoryId, bool? isAvailable,
        int page = 1)
    {
        var search = new BookSearchDto(
            title, author, isbn, categoryId, isAvailable, page, 12);

        var result = await bookService.SearchAsync(search);

        ViewBag.Categories = (await categoryService.GetAllAsync()).Value;
        ViewBag.Search     = search;

        return View(result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        var result = await bookService.GetByIdAsync(id);
        if (result.IsFailure)
            return NotFound();

        return View(result.Value);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        ViewBag.Categories =
            (await categoryService.GetAllAsync()).Value;
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(BookCreateDto dto)
    {
        var result = await bookService.CreateAsync(dto);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error;
            ViewBag.Categories =
                (await categoryService.GetAllAsync()).Value;
            return View(dto);
        }

        TempData["Success"] =
            $"تمت إضافة كتاب \"{dto.Title}\" بنجاح";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var result = await bookService.GetByIdAsync(id);
        if (result.IsFailure)
            return NotFound();

        ViewBag.Categories =
            (await categoryService.GetAllAsync()).Value;
        return View(result.Value);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id, BookUpdateDto dto)
    {
        var result = await bookService.UpdateAsync(id, dto);

        if (result.IsFailure)
        {
            TempData["Error"] = result.Error;
            ViewBag.Categories =
                (await categoryService.GetAllAsync()).Value;
            return View(dto);
        }

        TempData["Success"] = "تم تعديل الكتاب بنجاح";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await bookService.DeleteAsync(id);

        TempData[result.IsSuccess ? "Success" : "Error"] =
            result.IsSuccess ? "تم حذف الكتاب بنجاح" : result.Error;

        return RedirectToAction(nameof(Index));
    }
}