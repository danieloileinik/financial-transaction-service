using Application.Dto.Requests;
using Domain.Models;

namespace Application.Abstractions.Persistence;

public interface ITransactionsRepository
{
    Task AddAsync(Transaction transaction);

    Task<IReadOnlyList<Transaction>> GetHistoryAsync(
        Guid accountId,
        TransactionsRequest? request = null,
        CancellationToken ct = default);
}