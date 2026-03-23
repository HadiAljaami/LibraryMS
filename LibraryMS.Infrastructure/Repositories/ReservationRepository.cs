using LibraryMS.Domain.Entities;
using LibraryMS.Domain.Enums;
using LibraryMS.Domain.Interfaces.Repositories;
using LibraryMS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LibraryMS.Infrastructure.Repositories;

public class ReservationRepository(AppDbContext db)
    : GenericRepository<Reservation>(db), IReservationRepository
{
    public async Task<Reservation?> GetWithDetailsAsync(int id) =>
        await _dbSet
            .Include(r => r.Book)
            .Include(r => r.Member)
            .FirstOrDefaultAsync(r => r.Id == id);

    public async Task<IEnumerable<Reservation>> GetByMemberAsync(int memberId) =>
        await _dbSet
            .Include(r => r.Book)
            .Where(r => r.MemberId == memberId)
            .OrderByDescending(r => r.ReservationDate)
            .ToListAsync();

    public async Task<IEnumerable<Reservation>> GetByBookAsync(int bookId) =>
        await _dbSet
            .Include(r => r.Member)
            .Where(r => r.BookId == bookId)
            .OrderBy(r => r.ReservationDate)
            .ToListAsync();

    public async Task<IEnumerable<Reservation>> GetPendingByBookAsync(int bookId) =>
        await _dbSet
            .Include(r => r.Member)
            .Where(r => r.BookId == bookId &&
                        r.Status == ReservationStatus.Pending)
            .OrderBy(r => r.ReservationDate)
            .ToListAsync();

    public async Task<bool> HasActiveReservationAsync(int memberId, int bookId) =>
        await _dbSet.AnyAsync(r =>
            r.MemberId == memberId &&
            r.BookId == bookId &&
            (r.Status == ReservationStatus.Pending ||
             r.Status == ReservationStatus.Ready));

    public async Task<IEnumerable<Reservation>> GetExpiredReservationsAsync() =>
        await _dbSet
            .Where(r => r.Status == ReservationStatus.Pending &&
                        r.ExpiryDate < DateTime.UtcNow)
            .ToListAsync();

    public async Task<Reservation?> GetNextInQueueAsync(int bookId) =>
        await _dbSet
            .Include(r => r.Member)
            .Where(r => r.BookId == bookId &&
                        r.Status == ReservationStatus.Pending)
            .OrderBy(r => r.ReservationDate)
            .FirstOrDefaultAsync();
}