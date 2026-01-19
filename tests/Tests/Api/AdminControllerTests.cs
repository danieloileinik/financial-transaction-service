using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Application.Dto.Requests;
using Application.Dto.Responses;
using FluentAssertions;
using Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Tests.Api;

public class AdminControllerTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task CreateAccount_AsAdmin_Returns200WithAccountId()
    {
        // Arrange
        var client = factory.CreateAuthenticatedAdmin();

        // Act
        var response = await client.PostAsync("/api/admin/accounts", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var accountResponse = await response.Content.ReadFromJsonAsync<AccountResponse>();
        accountResponse.Should().NotBeNull();
        accountResponse.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task LockAccount_AsAdmin_Returns204()
    {
        // Arrange
        var accountId = await factory.CreateTestAccountAsync();
        var client = factory.CreateAuthenticatedAdmin();

        // Act
        var response = await client.PostAsync($"/api/admin/accounts/{accountId}/lock", null);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify in DB
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var account = await db.Accounts.FindAsync(accountId);
        account!.IsLocked.Should().BeTrue();
    }

    [Fact]
    public async Task DeleteAccount_AsAdmin_Returns204()
    {
        // Arrange
        var accountId = await factory.CreateTestAccountAsync();
        var client = factory.CreateAuthenticatedAdmin();

        // Act
        var response = await client.DeleteAsync($"/api/admin/accounts/{accountId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify in DB
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var account = await db.Accounts.FindAsync(accountId);
        account.Should().BeNull();
    }

    [Fact]
    public async Task AdjustBalance_PositiveAmount_DepositsAndReturns204()
    {
        // Arrange
        var accountId = await factory.CreateTestAccountAsync();
        var client = factory.CreateAuthenticatedAdmin();
        var request = new MoneyOperationRequest(1000m);

        // Act
        var response = await client.PutAsJsonAsync($"/api/admin/accounts/{accountId}/balance", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify balance
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var account = await db.Accounts.FindAsync(accountId);
        account!.Balance.Amount.Should().Be(1000m);
    }
}