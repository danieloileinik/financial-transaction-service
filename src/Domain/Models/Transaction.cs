using System;
using Domain.ValueObjects;

namespace Domain.Models;

public abstract class Transaction(Guid accountId, Money amount, DateTime timestamp)
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public Guid AccountId { get; private set; } = accountId;

    public Money Amount { get; private set; } = amount;

    public DateTime Timestamp { get; private set; } = timestamp;

    // protected Transaction()
    // {
    // } 
}

public class DepositTransaction(Guid accountId, Money amount, DateTime timestamp)
    : Transaction(accountId, amount, timestamp);

public class WithdrawTransaction(Guid accountId, Money amount, DateTime timestamp)
    : Transaction(accountId, amount, timestamp);