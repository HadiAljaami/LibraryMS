using LibraryMS.Domain.Common;
using LibraryMS.Domain.Entities;
using LibraryMS.Domain.Enums;
using LibraryMS.Domain.Interfaces.Repositories;
using LibraryMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryMS.Infrastructure.Repositories;

public class LoanRepository(AppDbContext db)
    : GenericRepository<Loan>(db), ILoanRepository
{
    public async Task<Loan?> GetWithDetailsAsync(int id) =>
        await _dbSet
            .Include(l => l.Book)
            .Include(l => l.BookCopy)
            .Include(l => l.Member)
            .FirstOrDefaultAsync(l => l.Id == id);

    public async Task<IEnumerable<Loan>> GetByMemberAsync(int memberId) =>
        await _dbSet
            .Include(l => l.Book)
            .Where(l => l.MemberId == memberId)
            .OrderByDescending(l => l.BorrowDate)
            .ToListAsync();

    public async Task<IEnumerable<Loan>> GetByBookAsync(int bookId) =>
        await _dbSet
            .Include(l => l.Member)
            .Where(l => l.BookId == bookId)
            .OrderByDescending(l => l.BorrowDate)
            .ToListAsync();

    public async Task<IEnumerable<Loan>> GetOverdueLoansAsync() =>
        await _dbSet
            .Include(l => l.Book)
            .Include(l => l.Member)
            .Where(l => l.Status == LoanStatus.Active &&
                        l.DueDate < DateTime.UtcNow)
            .OrderBy(l => l.DueDate)
            .ToListAsync();

    public async Task<IEnumerable<Loan>> GetActiveLoansByMemberAsync(int memberId) =>
        await _dbSet
            .Include(l => l.Book)
            .Where(l => l.MemberId == memberId &&
                        l.Status == LoanStatus.Active)
            .ToListAsync();

    public async Task<bool> HasActiveLoanAsync(int memberId, int bookId) =>
        await _dbSet.AnyAsync(l =>
            l.MemberId == memberId &&
            l.BookId == bookId &&
            l.Status == LoanStatus.Active);

    public async Task<PagedResult<Loan>> GetPagedWithDetailsAsync(
        int? memberId, int? bookId, LoanStatus? status,
        DateTime? fromDate, DateTime? toDate,
        int pageNumber, int pageSize)
    {
        var query = _dbSet
            .Include(l => l.Book)
            .Include(l => l.Member)
            .AsQueryable();

        if (memberId.HasValue)
            query = query.Where(l => l.MemberId == memberId);
        if (bookId.HasValue)
            query = query.Where(l => l.BookId == bookId);
        if (status.HasValue)
            query = query.Where(l => l.Status == status);
        if (fromDate.HasValue)
            query = query.Where(l => l.BorrowDate >= fromDate);
        if (toDate.HasValue)
            query = query.Where(l => l.BorrowDate <= toDate);

        var totalCount = await query.CountAsync();
        var items = await query
            .OrderByDescending(l => l.BorrowDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return new PagedResult<Loan>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<int> GetActiveLoanCountByMemberAsync(int memberId) =>
        await _dbSet.CountAsync(l =>
            l.MemberId == memberId &&
            l.Status == LoanStatus.Active);
}