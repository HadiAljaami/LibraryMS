using LibraryMS.Domain.Entities;

namespace LibraryMS.Domain.Interfaces.Repositories;

public interface ISystemSettingRepository
{
    Task<SystemSetting?> GetByKeyAsync(string key);
    Task<IEnumerable<SystemSetting>> GetByGroupAsync(string group);
    Task<IEnumerable<SystemSetting>> GetAllAsync();
    Task UpdateAsync(SystemSetting setting);
    Task<T> GetValueAsync<T>(string key, T defaultValue);
}