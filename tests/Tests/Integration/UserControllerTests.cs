using System;
using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Application.Dto.Requests;
using Application.Dto.Responses;
using Domain.ValueObjects;
using FluentAssertions;
using Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Tests.Api;

public class UserControllerTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task GetBalance_AsUser_Returns200WithBalance()
    {
        // Arrange
        var accountId = await factory.CreateTestAccountAsync();
        var client = factory.CreateAuthenticatedUser(accountId);

        // Act
        var response = await client.GetAsync("/api/account/balance");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var balance = await response.Content.ReadFromJsonAsync<BalanceResponse>();
        balance!.Balance.Should().Be(0m); // Initial balance
    }

    [Fact]
    public async Task Deposit_AsUser_Returns204AndUpdatesBalance()
    {
        // Arrange
        var accountId = await factory.CreateTestAccountAsync();
        var client = factory.CreateAuthenticatedUser(accountId);
        var request = new MoneyOperationRequest(500m);

        // Act
        var response = await client.PostAsJsonAsync("/api/account/deposit/atm", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify balance
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var account = await db.Accounts.FindAsync(accountId);
        account!.Balance.Amount.Should().Be(500m);
    }

    [Fact]
    public async Task Transfer_AsUser_ValidAmount_Returns204AndUpdatesBalances()
    {
        // Arrange
        var senderId = await factory.CreateTestAccountAsync();
        var receiverId = await factory.CreateTestAccountAsync();

        // Deposit to sender first (as admin or directly)
        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var sender = await db.Accounts.FindAsync(senderId);
        sender!.Deposit(Money.Create(1000m).Value);
        await db.SaveChangesAsync();

        var client = factory.CreateAuthenticatedUser(senderId);
        var request = new TransferRequest(receiverId, new MoneyOperationRequest(300m));

        // Act
        var response = await client.PostAsJsonAsync("/api/account/transfer", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Verify balances
        await db.Entry(sender).ReloadAsync();
        var receiver = await db.Accounts.FindAsync(receiverId);
        sender.Balance.Amount.Should().Be(700m);
        receiver!.Balance.Amount.Should().Be(300m);
    }

    [Fact]
    public async Task Transfer_AsUser_InsufficientFunds_Returns403()
    {
        // Arrange
        var senderId = await factory.CreateTestAccountAsync();
        var receiverId = await factory.CreateTestAccountAsync();
        var client = factory.CreateAuthenticatedUser(senderId);
        var request = new TransferRequest(receiverId, new MoneyOperationRequest(500m)); // Balance 0

        // Act
        var response = await client.PostAsJsonAsync("/api/account/transfer", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden); // Assuming ErrorHandler returns 403 for Forbidden
    }

    [Fact]
    public async Task Transfer_ToNonExistentAccount_Returns404()
    {
        // Arrange
        var senderId = await factory.CreateTestAccountAsync();

        using var scope = factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var sender = await db.Accounts.FindAsync(senderId);
        sender!.Deposit(Money.Create(1000m).Value);
        await db.SaveChangesAsync();

        var client = factory.CreateAuthenticatedUser(senderId);
        var fakeReceiverId = Guid.NewGuid();
        var request = new TransferRequest(fakeReceiverId, new MoneyOperationRequest(200m));

        // Act
        var response = await client.PostAsJsonAsync("/api/account/transfer", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var content = await response.Content.ReadAsStringAsync();
    }

    [Fact]
    public async Task GetTransactions_AsUser_Returns200WithEmptyHistoryInitially()
    {
        // Arrange
        var accountId = await factory.CreateTestAccountAsync();
        var client = factory.CreateAuthenticatedUser(accountId);

        // Act
        var response = await client.GetAsync("/api/account/transactions");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var history = await response.Content.ReadFromJsonAsync<TransactionsResponse>();
        history.Should().NotBeNull();
        history!.Transactions.Should().BeEmpty();
    }
}