using Application.Abstractions.Persistence;
using Application.Dto.Responses;
using Domain.Errors;
using ErrorOr;

namespace Application.UseCases.Accounts;

public class ViewBalanceHandler(IAccountRepository accountRepository)
{
    public async Task<ErrorOr<BalanceResponse>> Execute(Guid id, CancellationToken ct = default)
    {
        var balance = await accountRepository.GetBalanceAsync(id, ct);
        return balance is null ? AccountErrors.NotFound(id) : balance;
    }
}