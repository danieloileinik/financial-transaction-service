using System.Linq;
using ErrorOr;
using FinancialTransactionService.Domain.Errors;

namespace FinancialTransactionService.Domain.ValueObjects;

public readonly record struct PinCode
{
    private const int Length = 4;

    private PinCode(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static ErrorOr<PinCode> Create(string value)
    {
        return value.Length != Length || !value.All(char.IsDigit)
            ? ValueObjectErrors.InvalidPinFormat
            : new PinCode(value);
    }
}