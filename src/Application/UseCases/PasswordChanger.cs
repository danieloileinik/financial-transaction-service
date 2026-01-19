using Application.Abstractions.Persistence;
using Application.Dto.Requests;
using Domain;
using Domain.Errors;
using ErrorOr;

namespace Application.UseCases;

public class PasswordChanger(
    IAccountRepository accountRepository,
    IUnitOfWork unitOfWork,
    IPasswordHasher passwordHasher)
{
    public async Task<ErrorOr<Success>> Execute(
        Guid accountId,
        ChangePasswordRequest request,
        CancellationToken ct = default)
    {
        var account = await accountRepository.GetByIdAsync(accountId, ct);
        if (account is null) return AccountErrors.NotFound(accountId);

        var result = account.ChangePassword(request.OldPassword, request.NewPassword, passwordHasher);
        if (result.IsError) return result.FirstError;

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success;
    }
}