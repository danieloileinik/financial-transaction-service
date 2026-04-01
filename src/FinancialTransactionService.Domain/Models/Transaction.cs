using System;
using FinancialTransactionService.Domain.ValueObjects;

namespace FinancialTransactionService.Domain.Models;

public abstract class Transaction(Guid accountId, Money amount, DateTimeOffset timestamp)
{
    public Guid Id { get; private set; } = Guid.NewGuid();

    public Guid AccountId { get; private set; } = accountId;

    public Money Amount { get; private set; } = amount;

    public DateTimeOffset Timestamp { get; private set; } = timestamp;
}

public class DepositTransaction(Guid accountId, Money amount, DateTimeOffset timestamp)
    : Transaction(accountId, amount, timestamp);

public class WithdrawTransaction(Guid accountId, Money amount, DateTimeOffset timestamp)
    : Transaction(accountId, amount, timestamp);