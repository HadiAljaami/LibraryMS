using LibraryMS.Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LibraryMS.Web.Controllers;

[Authorize(Roles = "Admin")]
public class SettingsController(
    ISystemSettingRepository settingRepo) : Controller
{
    public async Task<IActionResult> Index()
    {
        var settings = await settingRepo.GetAllAsync();
        return View(settings);
    }

    [HttpPost]
    public async Task<IActionResult> Update(string key, string value)
    {
        var setting = await settingRepo.GetByKeyAsync(key);
        if (setting is null)
        {
            TempData["Error"] = "الإعداد غير موجود";
            return RedirectToAction(nameof(Index));
        }

        setting.Value     = value;
        setting.UpdatedAt = DateTime.UtcNow;
        await settingRepo.UpdateAsync(setting);

        TempData["Success"] = "تم تحديث الإعداد بنجاح";
        return RedirectToAction(nameof(Index));
    }
}