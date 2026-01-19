namespace Application.Abstractions.Security;

public interface ITokenService
{
    public string GenerateUserToken(Guid accountId, string role = "User");

    public string GenerateAdminToken();
}