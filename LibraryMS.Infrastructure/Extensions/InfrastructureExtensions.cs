using LibraryMS.Domain.Interfaces.Repositories;
using LibraryMS.Infrastructure.Audit;
using LibraryMS.Infrastructure.Data;
using LibraryMS.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryMS.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Database
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("Default"),
                b => b.MigrationsAssembly(
                    typeof(AppDbContext).Assembly.FullName)));

        // Repositories
        services.AddScoped(typeof(IGenericRepository<>),
            typeof(GenericRepository<>));
        services.AddScoped<IBookRepository, BookRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IMemberRepository, MemberRepository>();
        services.AddScoped<ILoanRepository, LoanRepository>();
        services.AddScoped<IReservationRepository, ReservationRepository>();
        services.AddScoped<ISystemSettingRepository, SystemSettingRepository>();
services.AddScoped<IFinePaymentRepository, FinePaymentRepository>();
        // Audit
        services.AddScoped<AuditService>();
        

        return services;
    }
}