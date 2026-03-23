using FluentValidation;
using LibraryMS.Application.DTOs.Loans;

namespace LibraryMS.Application.Validators;

public class LoanCreateValidator : AbstractValidator<LoanCreateDto>
{
    public LoanCreateValidator()
    {
        RuleFor(x => x.BookId)
            .GreaterThan(0).WithMessage("الكتاب مطلوب");

        RuleFor(x => x.MemberId)
            .GreaterThan(0).WithMessage("العضو مطلوب");

        RuleFor(x => x.DurationDays)
            .InclusiveBetween(1, 90)
            .WithMessage("مدة الاستعارة يجب أن تكون بين 1 و 90 يوم");
    }
}