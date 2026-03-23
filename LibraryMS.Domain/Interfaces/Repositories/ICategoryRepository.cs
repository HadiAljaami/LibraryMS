using LibraryMS.Domain.Entities;

namespace LibraryMS.Domain.Interfaces.Repositories;

public interface ICategoryRepository : IGenericRepository<Category>
{
    Task<Category?> GetWithBooksAsync(int id);
    Task<bool> IsNameUniqueAsync(string name, int? excludeId = null);
    Task<IEnumerable<Category>> GetAllWithBookCountAsync();
}