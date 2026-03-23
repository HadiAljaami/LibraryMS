using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace LibraryMS.Web.Controllers;

public class AccountController(
    SignInManager<IdentityUser> signInManager,
    UserManager<IdentityUser> userManager) : Controller
{
    [HttpGet]
    public IActionResult Login() =>
        User.Identity!.IsAuthenticated
            ? RedirectToAction("Index", "Home")
            : View();

    [HttpPost]
    public async Task<IActionResult> Login(
        string email, string password, bool rememberMe)
    {
        if (string.IsNullOrEmpty(email) ||
            string.IsNullOrEmpty(password))
        {
            ModelState.AddModelError("",
                "البريد الإلكتروني وكلمة المرور مطلوبان");
            return View();
        }

        var result = await signInManager.PasswordSignInAsync(
            email, password, rememberMe, lockoutOnFailure: true);

        if (result.Succeeded)
            return RedirectToAction("Index", "Home");

        if (result.IsLockedOut)
        {
            ModelState.AddModelError("",
                "الحساب مقفل مؤقتاً. حاول بعد 15 دقيقة");
            return View();
        }

        ModelState.AddModelError("",
            "البريد الإلكتروني أو كلمة المرور غير صحيحة");
        return View();
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return RedirectToAction("Login");
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> ChangePassword()
    {
        var user = await userManager.GetUserAsync(User);
        ViewBag.Email = user?.Email;
        return View();
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> ChangePassword(
        string currentPassword,
        string newPassword,
        string confirmPassword)
    {
        if (newPassword != confirmPassword)
        {
            TempData["Error"] = "كلمة المرور الجديدة غير متطابقة";
            return RedirectToAction(nameof(ChangePassword));
        }

        var user = await userManager.GetUserAsync(User);
        if (user is null)
            return RedirectToAction("Login");

        var result = await userManager.ChangePasswordAsync(
            user, currentPassword, newPassword);

        if (result.Succeeded)
        {
            TempData["Success"] = "تم تغيير كلمة المرور بنجاح";
            return RedirectToAction("Index", "Home");
        }

        TempData["Error"] = string.Join("، ",
            result.Errors.Select(e => e.Description));
        return RedirectToAction(nameof(ChangePassword));
    }

    public IActionResult AccessDenied() => View();
}