using Application.Abstractions.Persistence;
using Application.Dto.Requests;
using Application.Extensions;
using Domain;
using Domain.Errors;
using ErrorOr;

namespace Application.UseCases;

public class AccountAccessor(IAccountRepository accountRepository, IPasswordHasher passwordHasher)
{
    public async Task<ErrorOr<Success>> GetFromAtmAsync(
        UserAuthRequest.UserAtmAuthRequest request,
        CancellationToken ct = default)
    {
        var account = await accountRepository.GetByIdAsync(request.AccountId, ct);

        var pin = request.GetPinCode();
        if (pin.IsError) return ValueObjectErrors.InvalidPinFormat;

        if (account == null || !account.VerifyPin(pin.Value)) return AccountErrors.IncorrectPin;
        return Result.Success;
    }

    public async Task<ErrorOr<Success>> GetOnline(
        UserAuthRequest.UserOnlineAuthRequest request,
        CancellationToken ct = default)
    {
        var account = await accountRepository.GetByIdAsync(request.AccountId, ct);

        if (account == null || !account.VerifyPassword(request.Password, passwordHasher))
            return AccountErrors.IncorrectPassword;
        return Result.Success;
    }
}