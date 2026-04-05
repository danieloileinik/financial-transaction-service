using ErrorOr;
using FinancialTransactionService.Application.Abstractions.Persistence;
using FinancialTransactionService.Application.Dto.Requests;
using FinancialTransactionService.Application.Dto.Responses;
using FinancialTransactionService.Application.Extensions;
using FinancialTransactionService.Domain;
using FinancialTransactionService.Domain.Models;
using FinancialTransactionService.Domain.ValueObjects;

namespace FinancialTransactionService.Application.UseCases.Accounts;

public class CreateAccountHandler(
    IAccountRepository accountRepository,
    IPasswordHasher passwordHasher,
    IUnitOfWork unitOfWork)
{
    public async Task<ErrorOr<AccountResponse>> Execute(
        RegisterRequest request,
        CancellationToken ct = default)
    {
        var pin = PinCode.Create(request.Pin);
        if (pin.IsError) return pin.FirstError;

        var account = new Account();
        account.SetPin(pin.Value);
        account.SetPassword(request.Password, passwordHasher);

        accountRepository.Add(account);
        await unitOfWork.SaveChangesAsync(ct);

        return account.ToResponse();
    }

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