using LibraryMS.Domain.Entities;
using LibraryMS.Domain.Enums;

namespace LibraryMS.Domain.Interfaces.Repositories;

public interface IReservationRepository : IGenericRepository<Reservation>
{
    Task<Reservation?> GetWithDetailsAsync(int id);
    Task<IEnumerable<Reservation>> GetByMemberAsync(int memberId);
    Task<IEnumerable<Reservation>> GetByBookAsync(int bookId);
    Task<IEnumerable<Reservation>> GetPendingByBookAsync(int bookId);
    Task<bool> HasActiveReservationAsync(int memberId, int bookId);
    Task<IEnumerable<Reservation>> GetExpiredReservationsAsync();
    Task<Reservation?> GetNextInQueueAsync(int bookId);
}