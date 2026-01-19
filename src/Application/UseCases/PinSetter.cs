using Application.Abstractions.Persistence;
using Application.Dto.Requests;
using Application.Extensions;
using Domain.Errors;
using ErrorOr;

namespace Application.UseCases;

public class PinSetter(IAccountRepository accountRepository, IUnitOfWork unitOfWork)
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