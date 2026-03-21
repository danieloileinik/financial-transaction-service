#nullable enable
using System;
using System.Threading.Tasks;
using FinancialTransactionService.Application.Abstractions.Security;
using FinancialTransactionService.Infrastructure.Persistence;
using FinancialTransactionService.Infrastructure.Security;
using FinancialTransactionService.Presentation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Testcontainers.PostgreSql;
using Xunit;

namespace FinancialTransactionService.Tests;

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
    }

    public new async Task DisposeAsync()
    {
        if (_postgresContainer != null) await _postgresContainer.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
            services.RemoveAll(typeof(AppDbContext));

            var connectionString = _postgresContainer?.GetConnectionString()
                                   ?? "Host=localhost;Port=5432;Database=testdb;Username=testuser;Password=testpass";

            services.AddDbContextPool<AppDbContext>(options => { options.UseNpgsql(connectionString); });

            services.AddSingleton<ISystemPasswordProvider>(_ => new SystemPasswordProvider("TestAdminPassword"));
        });
    }
}