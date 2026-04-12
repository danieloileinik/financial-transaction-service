using FinancialTransactionService.Application.Abstractions.Security;
using FinancialTransactionService.Infrastructure.Persistence;
using FinancialTransactionService.Infrastructure.Security;
using FinancialTransactionService.Presentation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Xunit;

namespace FinancialTransactionService.IntegrationTests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private PostgreSqlContainer? _postgresContainer;

    [Obsolete("Obsolete")]
    public async Task InitializeAsync()
    {
        _postgresContainer = new PostgreSqlBuilder()
            .WithDatabase("testdb")
            .WithUsername("testuser")
            .WithPassword("testpass")
            .Build();

        await _postgresContainer.StartAsync();

        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        // await db.Database.EnsureDeletedAsync();
        await db.Database.MigrateAsync();
    }

    public new async Task DisposeAsync()
    {
        if (_postgresContainer != null) await _postgresContainer.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbDescriptors = services
                .Where(d => d.ServiceType.Namespace?.Contains("EntityFrameworkCore") == true
                            || d.ServiceType == typeof(AppDbContext))
                .ToList();

            foreach (var d in dbDescriptors) services.Remove(d);

            var connectionString = _postgresContainer?.GetConnectionString()
                                   ?? "Host=localhost;Port=5432;Database=testdb;Username=testuser;Password=testpass";

            services.AddDbContext<AppDbContext>(options => { options.UseNpgsql(connectionString); });

            services.AddSingleton<ISystemPasswordProvider>(_ => new SystemPasswordProvider("TestAdminPassword"));
        });
    }
}