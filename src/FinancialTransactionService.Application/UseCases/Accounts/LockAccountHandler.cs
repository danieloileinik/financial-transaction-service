using ErrorOr;
using FinancialTransactionService.Application.Abstractions.Persistence;
using FinancialTransactionService.Domain.Errors;

namespace FinancialTransactionService.Application.UseCases.Accounts;

public class LockAccountHandler(IAccountRepository accountRepository, IUnitOfWork unitOfWork)
{
    public async Task<ErrorOr<Success>> Lock(Guid accountId, CancellationToken ct = default)
    {
        var account = await accountRepository.GetByIdAsync(accountId, ct);
        if (account is null) return AccountErrors.NotFound(accountId);

        account.Lock();
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success;
    }

    public async Task<ErrorOr<Success>> Unlock(Guid accountId, CancellationToken ct = default)
    {
        var account = await accountRepository.GetByIdAsync(accountId, ct);
        if (account is null) return AccountErrors.NotFound(accountId);

        account.Unlock();
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success;
    }
}