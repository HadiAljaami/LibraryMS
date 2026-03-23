using LibraryMS.Domain.Entities;
using LibraryMS.Domain.Interfaces.Repositories;
using LibraryMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryMS.Infrastructure.Repositories;

public class FinePaymentRepository(AppDbContext db)
    : GenericRepository<FinePayment>(db), IFinePaymentRepository
{
    public async Task<IEnumerable<FinePayment>> GetByMemberAsync(int memberId) =>
        await _dbSet
            .Include(f => f.Member)
            .Where(f => f.MemberId == memberId)
            .OrderByDescending(f => f.PaymentDate)
            .ToListAsync();

    public async Task<decimal> GetTotalPaidByMemberAsync(int memberId) =>
        await _dbSet
            .Where(f => f.MemberId == memberId)
            .SumAsync(f => f.Amount);

    public async Task<IEnumerable<FinePayment>> GetByDateRangeAsync(
        DateTime fromDate, DateTime toDate) =>
        await _dbSet
            .Include(f => f.Member)
            .Where(f => f.PaymentDate >= fromDate &&
                        f.PaymentDate <= toDate)
            .OrderByDescending(f => f.PaymentDate)
            .ToListAsync();
}