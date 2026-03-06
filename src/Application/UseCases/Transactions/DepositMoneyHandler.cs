using Application.Abstractions.Persistence;
using Application.Dto.Requests;
using Application.Extensions;
using Domain.Errors;
using Domain.Models;
using ErrorOr;

namespace Application.UseCases.Transactions;

public class DepositMoneyHandler(
    IAccountRepository accountRepository,
    ITransactionsRepository transactionsRepository
)
{
    public async Task<ErrorOr<Success>> Execute(
        Guid accountId,
        MoneyOperationRequest request,
        CancellationToken ct = default)
    {
        var account = await accountRepository.GetByIdAsync(accountId, ct);
        if (account is null) return AccountErrors.NotFound(accountId);

        var money = request.ToDomain();
        if (money.IsError) return money.FirstError;

        var result = account.Deposit(money.Value);
        if (result.IsError) return result.FirstError;

        await transactionsRepository.AddAsync(new DepositTransaction(accountId, money.Value, DateTime.Now));
        return Result.Success;
    }
}