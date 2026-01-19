using System;
using Domain.Errors;
using Domain.ValueObjects;
using ErrorOr;

namespace Domain.Models;

public class Account
{
    private PasswordHash? _password;

    private PinCode? _pinCode;

    public Guid Id { get; } = Guid.NewGuid();

    public Money Balance { get; private set; }

    public bool IsLocked { get; private set; }

    public Account()
    {
        var money = Money.Create(0);
        Balance = money.Value;
    }

    public Account(Money balance)
    {
        Balance = balance;
    }

    public void Lock()
    {
        IsLocked = true;
    }

    public void Unlock()
    {
        IsLocked = false;
    }

    public void SetPin(PinCode pinCode)
    {
        _pinCode = pinCode;
    }

    public bool VerifyPin(PinCode pinCode)
    {
        return _pinCode?.Value == pinCode.Value;
    }

    public void SetPassword(string password, IPasswordHasher passwordHasher)
    {
        _password = new PasswordHash(passwordHasher.Hash(password));
    }

    public ErrorOr<Success> ChangePassword(string oldPassword, string newPassword, IPasswordHasher passwordHasher)
    {
        if (!VerifyPassword(oldPassword, passwordHasher)) return AccountErrors.IncorrectPassword;

        _password = new PasswordHash(passwordHasher.Hash(newPassword));
        return Result.Success;
    }

    public bool VerifyPassword(string password, IPasswordHasher passwordHasher)
    {
        var hash = _password?.Value;
        return hash is not null && passwordHasher.Verify(password, hash);
    }

    public ErrorOr<Success> Deposit(Money money)
    {
        if (IsLocked) return AccountErrors.Locked;

        Balance = Balance.IncreaseAmount(money);

        return Result.Success;
    }

    public ErrorOr<Success> Withdraw(Money money)
    {
        if (IsLocked) return AccountErrors.Locked;

        var balance = Balance.DecreaseAmount(money);
        if (balance.IsError) return AccountErrors.InsufficientFunds;
        Balance = balance.Value;

        return Result.Success;
    }
}