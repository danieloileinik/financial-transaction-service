using Application.Abstractions.Persistence;

namespace Infrastructure.Persistence;

public class EfUnitOfWork(AppDbContext dbContext) : IUnitOfWork
{
    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await dbContext.SaveChangesAsync(ct);
    }
}