using AutoMapper;
using LibraryMS.Application.DTOs.Reservations;
using LibraryMS.Domain.Common;
using LibraryMS.Domain.Entities;
using LibraryMS.Domain.Enums;
using LibraryMS.Domain.Interfaces.Repositories;

namespace LibraryMS.Application.Services;

public class ReservationService(
    IReservationRepository reservationRepo,
    IBookRepository bookRepo,
    IMemberRepository memberRepo,
    ISystemSettingRepository settingRepo,
    IMapper mapper)
{
    public async Task<Result<ReservationResponseDto>> CreateAsync(
        ReservationCreateDto dto, string createdBy)
    {
        var book = await bookRepo.GetByIdAsync(dto.BookId);
        if (book is null)
            return Result.Failure<ReservationResponseDto>("الكتاب غير موجود");

        if (book.AvailableCopies > 0)
            return Result.Failure<ReservationResponseDto>(
                "الكتاب متاح للاستعارة مباشرة");

        var member = await memberRepo.GetByIdAsync(dto.MemberId);
        if (member is null)
            return Result.Failure<ReservationResponseDto>("العضو غير موجود");

        if (!member.IsActive)
            return Result.Failure<ReservationResponseDto>("العضو غير نشط");

        if (await reservationRepo.HasActiveReservationAsync(
            dto.MemberId, dto.BookId))
            return Result.Failure<ReservationResponseDto>(
                "العضو لديه حجز مسبق لهذا الكتاب");

        var expiryDays = await settingRepo
            .GetValueAsync("ReservationExpiry", 3);

        var reservation = new Reservation
        {
            BookId          = dto.BookId,
            MemberId        = dto.MemberId,
            ReservationDate = DateTime.UtcNow,
            ExpiryDate      = DateTime.UtcNow.AddDays(expiryDays),
            Status          = ReservationStatus.Pending,
            CreatedBy       = createdBy,
            CreatedAt       = DateTime.UtcNow
        };

        await reservationRepo.AddAsync(reservation);

        var pending = await reservationRepo.GetPendingByBookAsync(dto.BookId);
        var position = pending.ToList().FindIndex(
            r => r.Id == reservation.Id) + 1;

        var result = mapper.Map<ReservationResponseDto>(reservation);
        return Result.Success(result with { QueuePosition = position });
    }

    public async Task<Result> CancelAsync(int id, string updatedBy)
    {
        var reservation = await reservationRepo.GetByIdAsync(id);
        if (reservation is null)
            return Result.Failure("الحجز غير موجود");

        if (reservation.Status != ReservationStatus.Pending &&
            reservation.Status != ReservationStatus.Ready)
            return Result.Failure("لا يمكن إلغاء هذا الحجز");

        reservation.Status    = ReservationStatus.Cancelled;
        reservation.UpdatedBy = updatedBy;
        reservation.UpdatedAt = DateTime.UtcNow;

        await reservationRepo.UpdateAsync(reservation);
        return Result.Success();
    }

    public async Task<Result<IEnumerable<ReservationResponseDto>>>
        GetByMemberAsync(int memberId)
    {
        var reservations = await reservationRepo.GetByMemberAsync(memberId);
        return Result.Success(
            reservations.Select(mapper.Map<ReservationResponseDto>));
    }

    public async Task ProcessExpiredReservationsAsync()
    {
        var expired = await reservationRepo.GetExpiredReservationsAsync();
        foreach (var reservation in expired)
        {
            reservation.Status    = ReservationStatus.Expired;
            reservation.UpdatedAt = DateTime.UtcNow;
            await reservationRepo.UpdateAsync(reservation);
        }
    }
}