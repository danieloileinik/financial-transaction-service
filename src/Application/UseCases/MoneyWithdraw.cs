using Application.Abstractions.Persistence;
using Application.Dto.Requests;
using Application.Extensions;
using Domain.Errors;
using Domain.Models;
using ErrorOr;

namespace Application.UseCases;

public class MoneyWithdraw(
    IAccountRepository accountRepository,
    ITransactionsRepository transactionsRepository,
    IUnitOfWork unitOfWork)
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

        var result = account.Withdraw(money.Value);
        if (result.IsError) return result.FirstError;

        transactionsRepository.Add(new WithdrawTransaction(accountId, money.Value, DateTime.Now));

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success;
    }
}