using Application.Abstractions.Persistence;
using Application.Dto.Requests;
using Application.Dto.Responses;
using Application.Extensions;
using Domain.Errors;
using ErrorOr;

namespace Application.UseCases;

public class TransactionsViewer(IAccountRepository accountRepository, ITransactionsRepository transactionsRepository)
{
    public async Task<ErrorOr<TransactionsResponse>> Execute(
        Guid accountId,
        TransactionsRequest? request = null,
        CancellationToken ct = default)
    {
        var account = await accountRepository.GetByIdAsync(accountId, ct);
        if (account == null) return AccountErrors.NotFound(accountId);

        var history = await transactionsRepository.GetHistoryAsync(accountId, request, ct);
        return history.ToResponse();
    }
}