using Domain.Errors;
using ErrorOr;

namespace Domain.ValueObjects;

public readonly record struct Money
{
    private const decimal MaxAmount = 5000000;

    public decimal Amount { get; }

    private Money(decimal amount)
    {
        Amount = amount;
    }

    public ErrorOr<Money> DecreaseAmount(Money amount)
    {
        return Create(Amount - amount.Amount);
    }

    public Money IncreaseAmount(Money amount)
    {
        return new Money(Amount + amount.Amount);
    }

    public static ErrorOr<Money> Create(decimal amount)
    {
        return amount is > MaxAmount or < 0 ? ValueObjectErrors.InvalidMoneyAmount : new Money(amount);
    }
}