using Application.Abstractions.Persistence;
using Application.Dto.Responses;
using Application.Extensions;
using Domain;
using Domain.Models;
using Domain.ValueObjects;
using ErrorOr;

namespace Application.UseCases;

public class AccountCreator(
    IAccountRepository accountRepository,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork)
{
    public async Task<ErrorOr<AccountResponse>> Execute(CancellationToken ct = default)
    {
        var account = new Account();

        var pin = PinCode.Create(Random.Shared.Next(1000, 10000).ToString());
        var password = Guid.NewGuid().ToString()[..12];
        account.SetPin(pin.Value);
        account.SetPassword(password, passwordHasher);

        accountRepository.Add(account);
        await unitOfWork.SaveChangesAsync(ct);

        return account.ToResponse();
    }
}