namespace Application.Dto.Requests;

public abstract record UserAuthRequest
{
    public sealed record UserAtmAuthRequest(Guid AccountId, string Pin) : UserAuthRequest;

    public sealed record UserOnlineAuthRequest(Guid AccountId, string Password) : UserAuthRequest;
}