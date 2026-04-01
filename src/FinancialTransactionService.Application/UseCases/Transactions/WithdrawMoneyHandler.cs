using ErrorOr;
using FinancialTransactionService.Application.Abstractions.Persistence;
using FinancialTransactionService.Application.Dto.Requests;
using FinancialTransactionService.Application.Extensions;
using FinancialTransactionService.Domain.Errors;
using FinancialTransactionService.Domain.Models;

namespace FinancialTransactionService.Application.UseCases.Transactions;

public class WithdrawMoneyHandler(
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

        transactionsRepository.Add(new WithdrawTransaction(accountId, money.Value, DateTimeOffset.UtcNow));

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success;
    }
}