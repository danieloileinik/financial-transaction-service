using Application.Dto.Responses;
using Domain.Models;
using Domain.ValueObjects;

namespace Application.Extensions;

public static class DomainToResponseExtensions
{
    public static BalanceResponse ToResponse(this Money money)
    {
        return new BalanceResponse(money.Amount);
    }

    public static AccountResponse ToResponse(this Account account)
    {
        return new AccountResponse(account.Id);
    }

    public static TransactionsResponse ToResponse(this IReadOnlyList<Transaction> transactionsHistory)
    {
        return new TransactionsResponse(transactionsHistory.Select(ToResponse).ToList());
    }

    private static TransactionsResponse.TransactionResponse ToResponse(this Transaction transaction)
    {
        return transaction switch
        {
            DepositTransaction d => new TransactionsResponse.TransactionResponse(
                "Deposit",
                d.Amount.Amount,
                d.Timestamp),
            WithdrawTransaction w => new TransactionsResponse.TransactionResponse(
                "Withdraw",
                w.Amount.Amount,
                w.Timestamp),
            _ => throw new ArgumentOutOfRangeException(nameof(transaction))
        };
    }
}