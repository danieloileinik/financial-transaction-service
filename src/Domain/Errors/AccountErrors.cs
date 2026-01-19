using System;
using ErrorOr;

namespace Domain.Errors;

public static class AccountErrors
{
    public static Error InsufficientFunds => Error.Forbidden("Insufficient funds");

    public static Error Locked => Error.Forbidden("Account is locked");

    public static Error IncorrectPassword => Error.Unauthorized("Incorrect password");

    public static Error IncorrectPin => Error.Unauthorized("Incorrect pin code");

    public static Error NotFound(Guid id)
    {
        var accountId = id.ToString();
        return Error.NotFound($"Account with ID:{accountId} not found");
    }
}