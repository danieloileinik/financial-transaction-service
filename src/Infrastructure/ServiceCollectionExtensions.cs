using Application.Abstractions.Persistence;
using Application.Abstractions.Security;
using Domain;
using Infrastructure.Persistence;
using Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

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