using ErrorOr;
using FinancialTransactionService.Application.Abstractions.Persistence;
using FinancialTransactionService.Domain.Errors;

namespace FinancialTransactionService.Application.UseCases.Accounts;

public class DeleteAccountHandler(IAccountRepository accountRepository)
{
    public async Task<ErrorOr<Success>> Execute(Guid accountId)
    {
        var account = await accountRepository.GetByIdAsync(accountId);
        if (account is null) return AccountErrors.NotFound(accountId);

        await accountRepository.DeleteAsync(accountId);
        return Result.Success;
    }
}