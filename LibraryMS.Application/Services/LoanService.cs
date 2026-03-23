using AutoMapper;
using LibraryMS.Application.DTOs.Loans;
using LibraryMS.Application.Validators;
using LibraryMS.Domain.Common;
using LibraryMS.Domain.Entities;
using LibraryMS.Domain.Enums;
using LibraryMS.Domain.Interfaces.Repositories;

namespace LibraryMS.Application.Services;

public class LoanService(
    ILoanRepository loanRepo,
    IBookRepository bookRepo,
    IMemberRepository memberRepo,
    ISystemSettingRepository settingRepo,
    IMapper mapper)
{
    public async Task<Result<PagedResult<LoanResponseDto>>> SearchAsync(
        LoanSearchDto search)
    {
        var result = await loanRepo.GetPagedWithDetailsAsync(
            search.MemberId, search.BookId, search.Status,
            search.FromDate, search.ToDate,
            search.PageNumber, search.PageSize);

        var mapped = new PagedResult<LoanResponseDto>(
            result.Items.Select(mapper.Map<LoanResponseDto>),
            result.TotalCount, result.PageNumber, result.PageSize);

        return Result.Success(mapped);
    }

    public async Task<Result<LoanResponseDto>> GetByIdAsync(int id)
    {
        var loan = await loanRepo.GetWithDetailsAsync(id);
        if (loan is null)
            return Result.Failure<LoanResponseDto>("الاستعارة غير موجودة");

        return Result.Success(mapper.Map<LoanResponseDto>(loan));
    }

    public async Task<Result<LoanResponseDto>> BorrowAsync(
        LoanCreateDto dto, string createdBy)
    {
        // Validate
        var validator = new LoanCreateValidator();
        var validation = await validator.ValidateAsync(dto);
        if (!validation.IsValid)
            return Result.Failure<LoanResponseDto>(
                string.Join("، ", validation.Errors.Select(e => e.ErrorMessage)));

        // Check book
        var book = await bookRepo.GetWithDetailsAsync(dto.BookId);
        if (book is null)
            return Result.Failure<LoanResponseDto>("الكتاب غير موجود");

        if (book.AvailableCopies <= 0)
            return Result.Failure<LoanResponseDto>(
                "لا توجد نسخ متاحة من هذا الكتاب");

        // Check member
        var member = await memberRepo.GetByIdAsync(dto.MemberId);
        if (member is null)
            return Result.Failure<LoanResponseDto>("العضو غير موجود");

        if (!member.IsActive)
            return Result.Failure<LoanResponseDto>("العضو غير نشط");

        // Check already borrowed
        if (await loanRepo.HasActiveLoanAsync(dto.MemberId, dto.BookId))
            return Result.Failure<LoanResponseDto>(
                "العضو لديه نسخة من هذا الكتاب بالفعل");

        // Check max loans
        var maxLoans = await settingRepo
            .GetValueAsync("MaxLoansPerMember", 5);
        var activeLoans = await loanRepo
            .GetActiveLoanCountByMemberAsync(dto.MemberId);

        if (activeLoans >= maxLoans)
            return Result.Failure<LoanResponseDto>(
                $"العضو وصل للحد الأقصى من الاستعارات ({maxLoans})");

        // Get available copy
        var copy = await bookRepo.GetWithDetailsAsync(dto.BookId);
        var availableCopy = copy!.Copies
            .FirstOrDefault(c => c.Status == CopyStatus.Available);

        if (availableCopy is null)
            return Result.Failure<LoanResponseDto>("لا توجد نسخ متاحة");

        // Create loan
        var loan = new Loan
        {
            BookId      = dto.BookId,
            BookCopyId  = availableCopy.Id,
            MemberId    = dto.MemberId,
            BorrowDate  = DateTime.UtcNow,
            DueDate     = DateTime.UtcNow.AddDays(dto.DurationDays),
            Status      = LoanStatus.Active,
            CreatedBy   = createdBy,
            CreatedAt   = DateTime.UtcNow
        };

        // Update book & copy
        book.AvailableCopies--;
        availableCopy.Status = CopyStatus.Borrowed;

        await loanRepo.AddAsync(loan);
        await bookRepo.UpdateAsync(book);

        var created = await loanRepo.GetWithDetailsAsync(loan.Id);
        return Result.Success(mapper.Map<LoanResponseDto>(created!));
    }

    public async Task<Result<LoanResponseDto>> ReturnAsync(
        int loanId, string updatedBy)
    {
        var loan = await loanRepo.GetWithDetailsAsync(loanId);
        if (loan is null)
            return Result.Failure<LoanResponseDto>("الاستعارة غير موجودة");

        if (loan.Status != LoanStatus.Active &&
            loan.Status != LoanStatus.Overdue)
            return Result.Failure<LoanResponseDto>("هذه الاستعارة مُرجعة مسبقاً");

        // Calculate fine
        var dailyFine = await settingRepo
            .GetValueAsync("DailyFineAmount", 1m);

        loan.ReturnDate = DateTime.UtcNow;
        loan.Status     = LoanStatus.Returned;
        loan.UpdatedBy  = updatedBy;
        loan.UpdatedAt  = DateTime.UtcNow;

        if (DateTime.UtcNow > loan.DueDate)
        {
            var overdueDays = (DateTime.UtcNow - loan.DueDate).Days;
            loan.FineAmount = overdueDays * dailyFine;

            // Update member fines
            loan.Member.TotalFines += loan.FineAmount;
            await memberRepo.UpdateAsync(loan.Member);
        }

        // Update book & copy
        loan.Book.AvailableCopies++;
        loan.BookCopy.Status = CopyStatus.Available;

        await loanRepo.UpdateAsync(loan);
        await bookRepo.UpdateAsync(loan.Book);

        return Result.Success(mapper.Map<LoanResponseDto>(loan));
    }

    public async Task<Result<LoanResponseDto>> RenewAsync(
        LoanRenewDto dto, string updatedBy)
    {
        var loan = await loanRepo.GetWithDetailsAsync(dto.LoanId);
        if (loan is null)
            return Result.Failure<LoanResponseDto>("الاستعارة غير موجودة");

        if (loan.Status != LoanStatus.Active)
            return Result.Failure<LoanResponseDto>(
                "لا يمكن تجديد استعارة غير نشطة");

        var maxRenewals = await settingRepo
            .GetValueAsync("MaxRenewals", 1);

        if (loan.RenewalCount >= maxRenewals)
            return Result.Failure<LoanResponseDto>(
                $"وصلت للحد الأقصى من التجديدات ({maxRenewals})");

        loan.DueDate       = loan.DueDate.AddDays(dto.AdditionalDays);
        loan.RenewalCount++;
        loan.Status        = LoanStatus.Renewed;
        loan.UpdatedBy     = updatedBy;
        loan.UpdatedAt     = DateTime.UtcNow;

        await loanRepo.UpdateAsync(loan);
        return Result.Success(mapper.Map<LoanResponseDto>(loan));
    }

    public async Task<Result<IEnumerable<LoanResponseDto>>> GetOverdueAsync()
    {
        var loans = await loanRepo.GetOverdueLoansAsync();
        return Result.Success(loans.Select(mapper.Map<LoanResponseDto>));
    }
}