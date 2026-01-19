using Application.Dto.Responses;
using Domain.Models;

namespace Application.Abstractions.Persistence;

public interface IAccountRepository
{
    void Update(Account account);

    void Add(Account account);

    Task DeleteAsync(Guid id);

    Task<BalanceResponse?> GetBalanceAsync(Guid id, CancellationToken ct = default);

    Task<Account?> GetByIdAsync(Guid id, CancellationToken ct = default);
}