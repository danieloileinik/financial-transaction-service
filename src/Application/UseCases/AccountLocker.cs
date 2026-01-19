using Application.Abstractions.Persistence;
using Domain.Errors;
using ErrorOr;

namespace Application.UseCases;

public class AccountLocker(IAccountRepository accountRepository, IUnitOfWork unitOfWork)
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