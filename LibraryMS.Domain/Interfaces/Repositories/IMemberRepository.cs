using LibraryMS.Domain.Common;
using LibraryMS.Domain.Entities;

namespace LibraryMS.Domain.Interfaces.Repositories;

public interface IMemberRepository : IGenericRepository<Member>
{
    Task<Member?> GetByEmailAsync(string email);
    Task<Member?> GetByMembershipNumberAsync(string membershipNumber);
    Task<Member?> GetWithLoansAsync(int id);
    Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null);
    Task<PagedResult<Member>> SearchAsync(
        string? name,
        string? email,
        string? membershipNumber,
        bool? isActive,
        int pageNumber,
        int pageSize);
    Task<IEnumerable<Member>> GetMembersWithUnpaidFinesAsync();
}