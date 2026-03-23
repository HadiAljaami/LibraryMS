using FluentValidation;
using LibraryMS.Application.Mappings;
using LibraryMS.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryMS.Application.Common;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplication(
        this IServiceCollection services)
    {
        // AutoMapper
        services.AddAutoMapper(cfg =>
            cfg.AddProfile<MappingProfile>());

        // FluentValidation
        services.AddValidatorsFromAssembly(
            typeof(ApplicationExtensions).Assembly);

        // Services
        services.AddScoped<BookService>();
        services.AddScoped<CategoryService>();
        services.AddScoped<MemberService>();
        services.AddScoped<LoanService>();
        services.AddScoped<ReservationService>();
        services.AddScoped<FineService>();

        return services;
    }
}