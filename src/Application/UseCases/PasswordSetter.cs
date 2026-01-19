using Application.Abstractions.Persistence;
using Application.Dto.Requests;
using Domain;
using Domain.Errors;
using ErrorOr;

namespace Application.UseCases;

public class PasswordSetter(
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