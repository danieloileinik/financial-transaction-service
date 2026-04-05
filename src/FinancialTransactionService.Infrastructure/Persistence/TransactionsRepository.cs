using FinancialTransactionService.Application.Abstractions.Persistence;
using FinancialTransactionService.Application.Dto.Requests;
using FinancialTransactionService.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FinancialTransactionService.Infrastructure.Persistence;

public class TransactionsRepository(AppDbContext dbContext) : ITransactionsRepository
{
    public void Add(Transaction transaction)
    {
        dbContext.Transactions.Add(transaction);
    }

    public async Task<IReadOnlyList<Transaction>> GetHistoryAsync(
        Guid accountId,
        TransactionRequest? request = null,
        CancellationToken ct = default)
    {
        var query = dbContext
            .Transactions
            .AsNoTracking()
            .Where(t => t.AccountId == accountId);
        if (request is not null)
            query = query.Where(t => t.Timestamp >= request.From && t.Timestamp <= request.To);
        return await query.ToListAsync(ct);
    }
}