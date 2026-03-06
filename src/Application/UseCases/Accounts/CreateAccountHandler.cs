using Application.Abstractions.Persistence;
using Application.Dto.Responses;
using Application.Extensions;
using Domain;
using Domain.Models;
using Domain.ValueObjects;
using ErrorOr;

namespace Application.UseCases.Accounts;

public class CreateAccountHandler(
    IAccountRepository accountRepository,
    IPasswordHasher passwordHasher)
{
    public async Task<ErrorOr<AccountResponse>> Execute(CancellationToken ct = default)
    {
        var account = new Account();

        var pin = PinCode.Create(Random.Shared.Next(1000, 10000).ToString());
        var password = Guid.NewGuid().ToString()[..12];
        account.SetPin(pin.Value);
        account.SetPassword(password, passwordHasher);

        await accountRepository.AddAsync(account);

        return account.ToResponse();
    }
}