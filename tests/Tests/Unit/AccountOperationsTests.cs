using Domain;
using Domain.Errors;
using Domain.Models;
using Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Tests.Unit;

public class AccountTests
{
    private readonly IPasswordHasher _fakeHasher = new FakePasswordHasher();

    [Fact]
    public void Constructor_Default_ShouldCreateWithZeroBalanceAndUnlocked()
    {
        // Act
        var account = new Account();

        // Assert
        account.Balance.Amount.Should().Be(0m);
        account.IsLocked.Should().BeFalse();
        account.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void Constructor_WithBalance_ShouldSetProvidedBalance()
    {
        // Arrange
        var initialBalance = Money.Create(5000m).Value;

        // Act
        var account = new Account(initialBalance);

        // Assert
        account.Balance.Amount.Should().Be(5000m);
    }

    [Fact]
    public void Deposit_ValidAmount_ShouldIncreaseBalance()
    {
        // Arrange
        var account = new Account();
        var deposit = Money.Create(1000m).Value;

        // Act
        var result = account.Deposit(deposit);

        // Assert
        result.IsError.Should().BeFalse();
        account.Balance.Amount.Should().Be(1000m);
    }

    [Fact]
    public void Deposit_LockedAccount_ShouldReturnLockedError()
    {
        // Arrange
        var account = new Account();
        account.Lock();

        // Act
        var result = account.Deposit(Money.Create(100m).Value);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(AccountErrors.Locked);
        account.Balance.Amount.Should().Be(0m);
    }

    [Fact]
    public void Withdraw_ValidAmount_ShouldDecreaseBalance()
    {
        // Arrange
        var account = new Account();
        account.Deposit(Money.Create(2000m).Value);

        // Act
        var result = account.Withdraw(Money.Create(700m).Value);

        // Assert
        result.IsError.Should().BeFalse();
        account.Balance.Amount.Should().Be(1300m);
    }

    [Fact]
    public void Withdraw_InsufficientFunds_ShouldReturnErrorAndNotChangeBalance()
    {
        // Arrange
        var account = new Account();
        account.Deposit(Money.Create(500m).Value);

        // Act
        var result = account.Withdraw(Money.Create(1000m).Value);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(AccountErrors.InsufficientFunds);
        account.Balance.Amount.Should().Be(500m);
    }

    [Fact]
    public void Withdraw_LockedAccount_ShouldReturnLockedError()
    {
        // Arrange
        var account = new Account();
        account.Deposit(Money.Create(1000m).Value);
        account.Lock();

        // Act
        var result = account.Withdraw(Money.Create(200m).Value);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(AccountErrors.Locked);
        account.Balance.Amount.Should().Be(1000m);
    }

    [Fact]
    public void SetPin_ValidPin_ShouldSetPinCode()
    {
        // Arrange
        var account = new Account();
        var pin = PinCode.Create("1234").Value;

        // Act
        account.SetPin(pin);

        // Assert
        account.VerifyPin(pin).Should().BeTrue();
    }

    [Fact]
    public void VerifyPin_IncorrectPin_ShouldReturnFalse()
    {
        // Arrange
        var account = new Account();
        account.SetPin(PinCode.Create("5678").Value);

        // Act & Assert
        account.VerifyPin(PinCode.Create("1234").Value).Should().BeFalse();
    }

    [Fact]
    public void SetPassword_ShouldSetHashedPassword()
    {
        // Arrange
        var account = new Account();

        // Act
        account.SetPassword("MyStrongPass123", _fakeHasher);

        // Assert
        account.VerifyPassword("MyStrongPass123", _fakeHasher).Should().BeTrue();
    }

    [Fact]
    public void ChangePassword_CorrectOldPassword_ShouldUpdatePassword()
    {
        // Arrange
        var account = new Account();
        account.SetPassword("OldPass123", _fakeHasher);

        // Act
        var result = account.ChangePassword("OldPass123", "NewPass456", _fakeHasher);

        // Assert
        result.IsError.Should().BeFalse();
        account.VerifyPassword("NewPass456", _fakeHasher).Should().BeTrue();
        account.VerifyPassword("OldPass123", _fakeHasher).Should().BeFalse();
    }

    [Fact]
    public void ChangePassword_IncorrectOldPassword_ShouldReturnError()
    {
        // Arrange
        var account = new Account();
        account.SetPassword("OldPass123", _fakeHasher);

        // Act
        var result = account.ChangePassword("WrongOld", "NewPass", _fakeHasher);

        // Assert
        result.IsError.Should().BeTrue();
        result.FirstError.Should().Be(AccountErrors.IncorrectPassword);
        account.VerifyPassword("NewPass", _fakeHasher).Should().BeFalse();
    }

    [Fact]
    public void Lock_ShouldSetIsLockedToTrue()
    {
        var account = new Account();
        account.Lock();
        account.IsLocked.Should().BeTrue();
    }

    [Fact]
    public void Unlock_ShouldSetIsLockedToFalse()
    {
        var account = new Account();
        account.Lock();
        account.Unlock();
        account.IsLocked.Should().BeFalse();
    }
}

public class FakePasswordHasher : IPasswordHasher
{
    public string Hash(string password)
    {
        return $"HASHED_{password}";
    }

    public bool Verify(string password, string hash)
    {
        return hash == $"HASHED_{password}";
    }
}