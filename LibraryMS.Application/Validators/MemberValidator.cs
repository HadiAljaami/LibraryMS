using FluentValidation;
using LibraryMS.Application.DTOs.Members;

namespace LibraryMS.Application.Validators;

public class MemberCreateValidator : AbstractValidator<MemberCreateDto>
{
    public MemberCreateValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("الاسم الكامل مطلوب")
            .MaximumLength(200).WithMessage("الاسم لا يتجاوز 200 حرف");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("البريد الإلكتروني مطلوب")
            .EmailAddress().WithMessage("البريد الإلكتروني غير صحيح")
            .MaximumLength(256).WithMessage("البريد لا يتجاوز 256 حرف");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("رقم الهاتف مطلوب")
            .Matches(@"^05\d{8}$")
            .WithMessage("رقم الهاتف يجب أن يبدأ بـ 05 ويتكون من 10 أرقام");

        RuleFor(x => x.MembershipExpiry)
            .GreaterThan(DateTime.Now)
            .WithMessage("تاريخ انتهاء العضوية يجب أن يكون في المستقبل");
    }
}

public class MemberUpdateValidator : AbstractValidator<MemberUpdateDto>
{
    public MemberUpdateValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("الاسم الكامل مطلوب")
            .MaximumLength(200).WithMessage("الاسم لا يتجاوز 200 حرف");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("البريد الإلكتروني مطلوب")
            .EmailAddress().WithMessage("البريد الإلكتروني غير صحيح");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("رقم الهاتف مطلوب")
            .Matches(@"^05\d{8}$")
            .WithMessage("رقم الهاتف يجب أن يبدأ بـ 05 ويتكون من 10 أرقام");
    }
}