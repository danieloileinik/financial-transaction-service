using ErrorOr;
using FinancialTransactionService.Application.Abstractions.Persistence;
using FinancialTransactionService.Application.Dto.Requests;
using FinancialTransactionService.Application.Extensions;
using FinancialTransactionService.Domain.Errors;

namespace FinancialTransactionService.Application.UseCases.Accounts;

public class SetPinHandler(IAccountRepository accountRepository, IUnitOfWork unitOfWork)
{
    public async Task<ErrorOr<Success>> Execute(
        Guid accountId,
        SetPinCodeRequest request,
        CancellationToken ct = default)
    {
        var account = await accountRepository.GetByIdAsync(accountId, ct);
        if (account is null) return AccountErrors.NotFound(accountId);

        var pin = request.ToDomain();
        if (pin.IsError) return pin.FirstError;

        account.SetPin(pin.Value);
        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success;
    }
}