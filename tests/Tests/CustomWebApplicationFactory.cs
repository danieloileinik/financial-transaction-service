#nullable enable
using System.Data.Common;
using System.Threading.Tasks;
using Application.Abstractions.Security;
using Infrastructure.Persistence;
using Infrastructure.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Presentation;
using Xunit;

namespace Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private DbConnection? _connection;

    public async Task InitializeAsync()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();
    }

    public new async Task DisposeAsync()
    {
        if (_connection != null) await _connection.DisposeAsync();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.RemoveAll(typeof(DbContextOptions<AppDbContext>));
            services.RemoveAll(typeof(IDbContextPool<AppDbContext>));
            services.RemoveAll(typeof(IDbContextFactory<AppDbContext>));
            services.RemoveAll(typeof(IScopedDbContextLease<AppDbContext>));
            services.RemoveAll(typeof(AppDbContext));

            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();

            services.AddDbContext<AppDbContext>(options => { options.UseSqlite(_connection); });

            services.AddSingleton<ISystemPasswordProvider>(_ => new SystemPasswordProvider("TestAdminPassword"));
        });
    }
}