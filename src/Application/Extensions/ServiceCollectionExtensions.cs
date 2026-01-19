using Application.UseCases;
using Microsoft.Extensions.DependencyInjection;

namespace Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection serviceCollection)
    {
        serviceCollection
            .AddScoped<AccountAccessor>()
            .AddScoped<AccountBalanceViewer>()
            .AddScoped<AccountCreator>()
            .AddScoped<AccountDelete>()
            .AddScoped<AccountLocker>()
            .AddScoped<MoneyWithdraw>()
            .AddScoped<MoneyDeposit>()
            .AddScoped<MoneyTransfer>()
            .AddScoped<PasswordChanger>()
            .AddScoped<PasswordSetter>()
            .AddScoped<PinSetter>()
            .AddScoped<TransactionsViewer>();

        return serviceCollection;
    }
}