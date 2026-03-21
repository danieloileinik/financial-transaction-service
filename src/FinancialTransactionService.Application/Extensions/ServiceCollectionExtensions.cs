using FinancialTransactionService.Application.UseCases.Accounts;
using FinancialTransactionService.Application.UseCases.Transactions;
using Microsoft.Extensions.DependencyInjection;

namespace FinancialTransactionService.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddScoped<AccessAccountHandler>()
            .AddScoped<ViewBalanceHandler>()
            .AddScoped<CreateAccountHandler>()
            .AddScoped<DeleteAccountHandler>()
            .AddScoped<LockAccountHandler>()
            .AddScoped<WithdrawMoneyHandler>()
            .AddScoped<DepositMoneyHandler>()
            .AddScoped<TransferMoneyHandler>()
            .AddScoped<ChangePasswordHandler>()
            .AddScoped<SetPasswordHandler>()
            .AddScoped<SetPinHandler>()
            .AddScoped<ViewTransactionsHandler>();

        return serviceCollection;
    }
}