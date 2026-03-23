using LibraryMS.Domain.Common;
using LibraryMS.Domain.Entities;
using LibraryMS.Domain.Interfaces.Repositories;
using LibraryMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryMS.Infrastructure.Repositories;

public class MemberRepository(AppDbContext db)
    : GenericRepository<Member>(db), IMemberRepository
{
    public async Task<Member?> GetByEmailAsync(string email) =>
        await _dbSet.FirstOrDefaultAsync(m => m.Email == email);

    public async Task<Member?> GetByMembershipNumberAsync(string number) =>
        await _dbSet.FirstOrDefaultAsync(m => m.MembershipNumber == number);

    public async Task<Member?> GetWithLoansAsync(int id) =>
        await _dbSet
            .Include(m => m.Loans)
                .ThenInclude(l => l.Book)
            .Include(m => m.FinePayments)
            .FirstOrDefaultAsync(m => m.Id == id);

    public async Task<bool> IsEmailUniqueAsync(string email, int? excludeId = null)
    {
        var query = _dbSet.Where(m => m.Email == email);
        if (excludeId.HasValue)
            query = query.Where(m => m.Id != excludeId);
        return !await query.AnyAsync();
    }

    public async Task<PagedResult<Member>> SearchAsync(
        string? name,
        string? email,
        string? membershipNumber,
        bool? isActive,
        int pageNumber,
        int pageSize)
    {
        var query = _dbSet.AsQueryable();

        if (!string.IsNullOrEmpty(name))
            query = query.Where(m => m.FullName.Contains(name));

        if (!string.IsNullOrEmpty(email))
            query = query.Where(m => m.Email.Contains(email));

        if (!string.IsNullOrEmpty(membershipNumber))
            query = query.Where(m => m.MembershipNumber.Contains(membershipNumber));

        if (isActive.HasValue)
            query = query.Where(m => m.IsActive == isActive);

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderBy(m => m.FullName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Member>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<IEnumerable<Member>> GetMembersWithUnpaidFinesAsync() =>
        await _dbSet
            .Where(m => m.TotalFines > m.PaidFines)
            .OrderByDescending(m => m.TotalFines - m.PaidFines)
            .ToListAsync();
}