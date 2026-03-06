using System.Net;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Application.Dto.Requests;
using FluentAssertions;
using Xunit;

namespace Tests.Api;

public class AuthControllerTests(CustomWebApplicationFactory factory) : IClassFixture<CustomWebApplicationFactory>
{
    [Fact]
    public async Task UserAtmAuth_ValidPin_Returns200WithJwt()
    {
        // Arrange
        var accountId = await factory.CreateTestAccountAsync("1234");
        var client = factory.CreateClient();
        var request = new UserAuthRequest.UserAtmAuthRequest(accountId, "1234");

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/user/atm", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var token = await response.Content.ReadAsStringAsync();
        token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task UserAtmAuth_InvalidPin_Returns401()
    {
        // Arrange
        var accountId = await factory.CreateTestAccountAsync("1234");
        var client = factory.CreateClient();
        var request = new UserAuthRequest.UserAtmAuthRequest(accountId, "9999");

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/user/atm", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task UserOnlineAuth_ValidPassword_Returns200WithJwt()
    {
        // Arrange
        var accountId = await factory.CreateTestAccountAsync(password: "password");
        var client = factory.CreateClient();
        var request = new UserAuthRequest.UserOnlineAuthRequest(accountId, "password");

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/user/online", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var token = await response.Content.ReadAsStringAsync();
        token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task UserOnlineAuth_InvalidPassword_Returns401()
    {
        // Arrange
        var accountId = await factory.CreateTestAccountAsync(password: "password");
        var client = factory.CreateClient();
        var request = new UserAuthRequest.UserOnlineAuthRequest(accountId, "incorrect");

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/user/online", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task AdminAuth_ValidPassword_Returns200WithJwt()
    {
        // Arrange
        var client = factory.CreateClient();
        var request = new AdminAuthRequest("TestAdminPassword");

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/admin", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var token = await response.Content.ReadAsStringAsync();
        token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task AdminAuth_InvalidPassword_Returns401()
    {
        // Arrange
        var client = factory.CreateClient();
        var request = new AdminAuthRequest("WrongPassword");

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/admin", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}