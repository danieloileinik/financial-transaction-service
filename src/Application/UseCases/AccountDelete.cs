using Application.Abstractions.Persistence;
using Domain.Errors;
using ErrorOr;

namespace Application.UseCases;

public class AccountDelete(IAccountRepository accountRepository)
{
    public async Task<ErrorOr<Success>> Execute(Guid accountId)
    {
        var account = await accountRepository.GetByIdAsync(accountId);
        if (account is null) return AccountErrors.NotFound(accountId);

        await accountRepository.DeleteAsync(accountId);
        return Result.Success;
    }
}