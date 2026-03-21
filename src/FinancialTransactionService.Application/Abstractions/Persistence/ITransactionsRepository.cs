using FinancialTransactionService.Application.Dto.Requests;
using FinancialTransactionService.Domain.Models;

namespace FinancialTransactionService.Application.Abstractions.Persistence;

public interface ITransactionsRepository
{
    void Add(Transaction transaction);

    Task<IReadOnlyList<Transaction>> GetHistoryAsync(
        Guid accountId,
        TransactionsRequest? request = null,
        CancellationToken ct = default);
}