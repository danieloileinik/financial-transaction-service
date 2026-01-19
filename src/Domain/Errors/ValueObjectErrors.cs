using ErrorOr;

namespace Domain.Errors;

public static class ValueObjectErrors
{
    public static Error InvalidPinFormat => Error.Validation("Invalid pin code format");

    public static Error InvalidMoneyAmount => Error.Validation("The amount of money is out of range");
}