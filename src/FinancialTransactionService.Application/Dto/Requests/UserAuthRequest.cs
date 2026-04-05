namespace FinancialTransactionService.Application.Dto.Requests;

public abstract record UserAuthRequest
{
    public sealed record UserAtmAuthRequest(Guid AccountId, string Pin) : UserAuthRequest;

    public sealed record UserOnlineAuthRequest(Guid AccountId, string Password) : UserAuthRequest;
}
// public readonly record struct UserAuthRequest
// {
//     public readonly record struct UserAtmAuthRequest(Guid AccountId, string Pin);
//
//     public readonly  record  struct UserOnlineAuthRequest(Guid AccountId, string Password) ;
// }