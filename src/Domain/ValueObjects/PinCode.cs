using System.Linq;
using Domain.Errors;
using ErrorOr;

namespace Domain.ValueObjects;

public readonly record struct PinCode
{
    private const int Length = 4;

    public string Value { get; }

    private PinCode(string value)
    {
        Value = value;
    }

    public static ErrorOr<PinCode> Create(string value)
    {
        return value.Length != Length || !value.All(char.IsDigit)
            ? ValueObjectErrors.InvalidPinFormat
            : new PinCode(value);
    }
}