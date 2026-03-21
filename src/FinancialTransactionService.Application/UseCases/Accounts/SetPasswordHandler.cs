using ErrorOr;
using FinancialTransactionService.Application.Abstractions.Persistence;
using FinancialTransactionService.Application.Dto.Requests;
using FinancialTransactionService.Domain;
using FinancialTransactionService.Domain.Errors;

namespace FinancialTransactionService.Application.UseCases.Accounts;

public class SetPasswordHandler(
    IAccountRepository accountRepository,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork)
{
    public async Task<ErrorOr<Success>> Execute(
        Guid accountId,
        SetPasswordRequest request,
        CancellationToken ct = default)
    {
        var account = await accountRepository.GetByIdAsync(accountId, ct);
        if (account is null) return AccountErrors.NotFound(accountId);

        account.SetPassword(request.Password, passwordHasher);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success;
    }
}