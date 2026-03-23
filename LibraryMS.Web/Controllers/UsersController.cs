using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryMS.Web.Controllers;

[Authorize(Roles = "Admin")]

public class UsersController(
    UserManager<IdentityUser> userManager) : Controller
{
    public async Task<IActionResult> Index()
    {
        var users = await userManager.Users.ToListAsync();
        var result = new List<(IdentityUser User, string Role)>();

        foreach (var user in users)
        {
            var roles = await userManager.GetRolesAsync(user);
            result.Add((user, roles.FirstOrDefault() ?? "—"));
        }

        return View(result);
    }

    [HttpPost]
    public async Task<IActionResult> Create(
        string email, string password, string role)
    {
        if (await userManager.FindByEmailAsync(email) is not null)
        {
            TempData["Error"] = "البريد الإلكتروني مستخدم مسبقاً";
            return RedirectToAction(nameof(Index));
        }

        var user = new IdentityUser
        {
            UserName       = email,
            Email          = email,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            TempData["Error"] = string.Join("، ",
                result.Errors.Select(e => e.Description));
            return RedirectToAction(nameof(Index));
        }

        await userManager.AddToRoleAsync(user, role);
        TempData["Success"] = $"تم إنشاء المستخدم {email} بنجاح";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> ResetPassword(
        string id, string newPassword)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user is null) return NotFound();

        var token  = await userManager
            .GeneratePasswordResetTokenAsync(user);
        var result = await userManager
            .ResetPasswordAsync(user, token, newPassword);

        TempData[result.Succeeded ? "Success" : "Error"] =
            result.Succeeded
                ? "تم تغيير كلمة المرور بنجاح"
                : string.Join("، ",
                    result.Errors.Select(e => e.Description));

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await userManager.FindByIdAsync(id);
        if (user is null) return NotFound();

        if (user.UserName == User.Identity!.Name)
        {
            TempData["Error"] = "لا يمكنك حذف حسابك الخاص";
            return RedirectToAction(nameof(Index));
        }

        await userManager.DeleteAsync(user);
        TempData["Success"] = "تم حذف المستخدم بنجاح";
        return RedirectToAction(nameof(Index));
    }
}