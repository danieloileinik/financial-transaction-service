using FinancialTransactionService.Application.Dto.Responses;
using FinancialTransactionService.Domain.Models;

namespace FinancialTransactionService.Application.Abstractions.Persistence;

public interface IAccountRepository
{
    void Add(Account account);

    Task DeleteAsync(Guid id);

    Task<BalanceResponse?> GetBalanceAsync(Guid id, CancellationToken ct = default);

    Task<Account?> GetByIdAsync(Guid id, CancellationToken ct = default);
}