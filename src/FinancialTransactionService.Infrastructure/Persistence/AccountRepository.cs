using FinancialTransactionService.Application.Abstractions.Persistence;
using FinancialTransactionService.Application.Dto.Responses;
using FinancialTransactionService.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace FinancialTransactionService.Infrastructure.Persistence;

public class AccountRepository(AppDbContext dbContext) : IAccountRepository
{
    public void Add(Account account)
    {
        dbContext.Accounts.Add(account);
    }

    public async Task DeleteAsync(Guid id)
    {
        await dbContext.Accounts.Where(a => a.Id == id)
            .ExecuteUpdateAsync(s => s.SetProperty(
                a => EF.Property<bool>(a, "IsDeleted"),
                true));
    }

    public async Task<BalanceResponse?> GetBalanceAsync(Guid id, CancellationToken ct = default)
    {
        return await dbContext.Accounts
            .AsNoTracking()
            .Where(a => a.Id == id)
            .Select(a => new BalanceResponse(a.Balance.Amount))
            .FirstOrDefaultAsync(ct);
    }

    public async Task<Account?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        return await dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == id, ct);
    }
}