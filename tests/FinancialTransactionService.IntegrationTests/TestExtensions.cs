using System.Net.Http.Headers;
using FinancialTransactionService.Application.Abstractions.Security;
using FinancialTransactionService.Domain;
using FinancialTransactionService.Domain.Models;
using FinancialTransactionService.Domain.ValueObjects;
using FinancialTransactionService.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;

namespace FinancialTransactionService.IntegrationTests;

public static class TestExtensions
{
    extension(CustomWebApplicationFactory factory)
    {
        public HttpClient CreateAuthenticatedUser(Guid accountId)
        {
            var client = factory.CreateClient();

            using var scope = factory.Services.CreateScope();
            var tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();

            var token = tokenService.GenerateUserToken(accountId);

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        public HttpClient CreateAuthenticatedAdmin()
        {
            var client = factory.CreateClient();

            using var scope = factory.Services.CreateScope();
            var tokenService = scope.ServiceProvider.GetRequiredService<ITokenService>();

            var token = tokenService.GenerateAdminToken();

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            return client;
        }

        public async Task<Guid> CreateTestAccountAsync(
            string? pin = null,
            string? password = null)
        {
            using var scope = factory.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

            var account = new Account();
            if (pin != null)
            {
                var pinCode = PinCode.Create(pin).Value;
                account.SetPin(pinCode);
            }

            if (password != null) account.SetPassword(password, passwordHasher);

            db.Accounts.Add(account);
            await db.SaveChangesAsync();

            return account.Id;
        }
    }
}