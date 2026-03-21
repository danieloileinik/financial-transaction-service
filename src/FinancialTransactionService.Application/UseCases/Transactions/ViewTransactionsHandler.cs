using ErrorOr;
using FinancialTransactionService.Application.Abstractions.Persistence;
using FinancialTransactionService.Application.Dto.Requests;
using FinancialTransactionService.Application.Dto.Responses;
using FinancialTransactionService.Application.Extensions;
using FinancialTransactionService.Domain.Errors;

namespace FinancialTransactionService.Application.UseCases.Transactions;

public class ViewTransactionsHandler(
    IAccountRepository accountRepository,
    ITransactionsRepository transactionsRepository)
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