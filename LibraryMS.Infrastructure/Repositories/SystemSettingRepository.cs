using LibraryMS.Domain.Entities;
using LibraryMS.Domain.Interfaces.Repositories;
using LibraryMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryMS.Infrastructure.Repositories;

public class SystemSettingRepository(AppDbContext db)
    : ISystemSettingRepository
{
    public async Task<SystemSetting?> GetByKeyAsync(string key) =>
        await db.SystemSettings
            .FirstOrDefaultAsync(s => s.Key == key);

    public async Task<IEnumerable<SystemSetting>> GetByGroupAsync(
        string group) =>
        await db.SystemSettings
            .Where(s => s.Group == group)
            .OrderBy(s => s.Key)
            .ToListAsync();

    public async Task<IEnumerable<SystemSetting>> GetAllAsync() =>
        await db.SystemSettings
            .OrderBy(s => s.Group)
            .ThenBy(s => s.Key)
            .ToListAsync();

    public async Task UpdateAsync(SystemSetting setting)
    {
        db.SystemSettings.Update(setting);
        await db.SaveChangesAsync();
    }

    public async Task<T> GetValueAsync<T>(string key, T defaultValue)
    {
        var setting = await GetByKeyAsync(key);
        if (setting is null) return defaultValue;

        try
        {
            return (T)Convert.ChangeType(setting.Value, typeof(T));
        }
        catch
        {
            return defaultValue;
        }
    }
}