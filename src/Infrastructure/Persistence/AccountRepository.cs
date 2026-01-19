using Application.Abstractions.Persistence;
using Application.Dto.Responses;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class AccountRepository(AppDbContext dbContext) : IAccountRepository
{
    public void Update(Account account)
    {
        dbContext.Accounts.Update(account);
    }

    public void Add(Account account)
    {
        dbContext.Accounts.Add(account);
    }

    public async Task DeleteAsync(Guid id)
    {
        await dbContext.Accounts.Where(a => a.Id == id).ExecuteDeleteAsync();
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