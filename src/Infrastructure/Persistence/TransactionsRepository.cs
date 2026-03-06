using Application.Abstractions.Persistence;
using Application.Dto.Requests;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class TransactionsRepository(AppDbContext dbContext) : ITransactionsRepository
{
    public void Add(Transaction transaction)
    {
        dbContext.Transactions.Add(transaction);
    }

    public async Task<IReadOnlyList<Transaction>> GetHistoryAsync(
        Guid accountId,
        TransactionsRequest? request = null,
        CancellationToken ct = default)
    {
        var query = dbContext.Transactions.AsNoTracking()
            .Where(t => t.AccountId == accountId);
        if (request is not null) query = query.Where(t => t.Timestamp >= request.From && t.Timestamp <= request.To);

        return await query.ToListAsync(ct);
    }
}