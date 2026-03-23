using LibraryMS.Domain.Entities;
using LibraryMS.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace LibraryMS.Infrastructure.Identity;

public static class DbSeeder
{
    public static async Task SeedAsync(
        AppDbContext db,
        UserManager<IdentityUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        // Migrate
        await db.Database.MigrateAsync();

        // Roles
        string[] roles = ["Admin", "Librarian"];
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        // Admin User
        var adminEmail = "admin@libraryms.com";
        if (await userManager.FindByEmailAsync(adminEmail) is null)
        {
            var admin = new IdentityUser
            {
                UserName       = adminEmail,
                Email          = adminEmail,
                EmailConfirmed = true
            };
            await userManager.CreateAsync(admin, "Admin@123456");
            await userManager.AddToRoleAsync(admin, "Admin");
        }

        // Default Categories
        if (!await db.Categories.AnyAsync())
        {
            var categories = new List<Category>
            {
                new() { Name = "علوم وتقنية",    IconClass = "bi-cpu",          CreatedBy = "system" },
                new() { Name = "أدب وشعر",        IconClass = "bi-book",         CreatedBy = "system" },
                new() { Name = "تاريخ وحضارة",    IconClass = "bi-hourglass",    CreatedBy = "system" },
                new() { Name = "دين وفقه",         IconClass = "bi-moon-stars",   CreatedBy = "system" },
                new() { Name = "طب وصحة",          IconClass = "bi-heart-pulse",  CreatedBy = "system" },
                new() { Name = "اقتصاد وأعمال",   IconClass = "bi-graph-up",     CreatedBy = "system" },
                new() { Name = "فلسفة وفكر",       IconClass = "bi-lightbulb",    CreatedBy = "system" },
                new() { Name = "أطفال وناشئة",    IconClass = "bi-stars",        CreatedBy = "system" }
            };
            await db.Categories.AddRangeAsync(categories);
        }

        // Default System Settings
        if (!await db.SystemSettings.AnyAsync())
        {
            var settings = new List<SystemSetting>
            {
                new() { Key = "LibraryName",        Value = "نظام إدارة المكتبة", Group = "General",  CreatedBy = "system" },
                new() { Key = "DailyFineAmount",    Value = "1",                   Group = "Fines",    CreatedBy = "system" },
                new() { Key = "MaxLoanDays",        Value = "14",                  Group = "Loans",    CreatedBy = "system" },
                new() { Key = "MaxRenewals",        Value = "1",                   Group = "Loans",    CreatedBy = "system" },
                new() { Key = "MaxLoansPerMember",  Value = "5",                   Group = "Loans",    CreatedBy = "system" },
                new() { Key = "ReservationExpiry",  Value = "3",                   Group = "Reservations", CreatedBy = "system" }
            };
            await db.SystemSettings.AddRangeAsync(settings);
        }

        await db.SaveChangesAsync();
    }
}