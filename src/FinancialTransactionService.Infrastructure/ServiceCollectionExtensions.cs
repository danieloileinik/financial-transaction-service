using FinancialTransactionService.Application.Abstractions.Persistence;
using FinancialTransactionService.Application.Abstractions.Security;
using FinancialTransactionService.Domain;
using FinancialTransactionService.Infrastructure.Persistence;
using FinancialTransactionService.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FinancialTransactionService.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection serviceCollection,
        IConfiguration configuration)
    {
        serviceCollection
            .AddDbContextPool<AppDbContext>(options =>
            {
                options.UseSqlite(configuration.GetConnectionString("DefaultConnection"));
            })
            .AddScoped<IAccountRepository, AccountRepository>()
            .AddScoped<ITransactionsRepository, TransactionsRepository>()
            .AddSingleton<IPasswordHasher, PasswordHasher>()
            .AddSingleton<ITokenService, JwtTokenService>()
            .AddScoped<IUnitOfWork, EfUnitOfWork>();

        return serviceCollection;
    }
}