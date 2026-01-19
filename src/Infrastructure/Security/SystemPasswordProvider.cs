using Application.Abstractions.Security;

namespace Infrastructure.Security;

public class SystemPasswordProvider(string password) : ISystemPasswordProvider
{
    public string Password { get; } = password ?? throw new ArgumentNullException(nameof(password));
}