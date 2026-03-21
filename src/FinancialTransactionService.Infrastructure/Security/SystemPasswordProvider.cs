using FinancialTransactionService.Application.Abstractions.Security;

namespace FinancialTransactionService.Infrastructure.Security;

public class SystemPasswordProvider(string password) : ISystemPasswordProvider
{
    public string Password { get; } = password ?? throw new ArgumentNullException(nameof(password));
}