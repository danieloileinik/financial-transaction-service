namespace Application.Abstractions.Security;

public interface ISystemPasswordProvider
{
    string Password { get; }
}