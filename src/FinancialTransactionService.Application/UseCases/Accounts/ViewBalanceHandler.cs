using ErrorOr;
using FinancialTransactionService.Application.Abstractions.Persistence;
using FinancialTransactionService.Application.Dto.Responses;
using FinancialTransactionService.Domain.Errors;

namespace FinancialTransactionService.Application.UseCases.Accounts;

public class ViewBalanceHandler(IAccountRepository accountRepository)
{
    public async Task<ErrorOr<BalanceResponse>> Execute(Guid id, CancellationToken ct = default)
    {
        var balance = await accountRepository.GetBalanceAsync(id, ct);
        return balance is null ? AccountErrors.NotFound(id) : (ErrorOr<BalanceResponse>)balance;
    }
}