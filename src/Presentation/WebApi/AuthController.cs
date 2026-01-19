using Application.Abstractions.Security;
using Application.Dto.Requests;
using Application.UseCases;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.WebApi;

[ApiController]
[Route("/api/auth")]
public class AuthController(
    ISystemPasswordProvider passwordProvider,
    ITokenService tokenService,
    AccountAccessor accountAccessor) : ControllerBase
{
    [HttpPost("user/atm")]
    public async Task<IActionResult> GetUserJwt(
        [FromBody] UserAuthRequest.UserAtmAuthRequest request,
        [FromQuery] CancellationToken ct = default)
    {
        var result = await accountAccessor.GetFromAtmAsync(request, ct);
        if (result.IsError) return ErrorHandler.Handle(result);

        var token = tokenService.GenerateUserToken(request.AccountId);
        return Ok(token);
    }

    [HttpPost("user/online")]
    public async Task<IActionResult> GetUserJwt(
        [FromBody] UserAuthRequest.UserOnlineAuthRequest request,
        [FromQuery] CancellationToken ct = default)
    {
        var result = await accountAccessor.GetOnline(request, ct);
        if (result.IsError) return ErrorHandler.Handle(result);

        var token = tokenService.GenerateUserToken(request.AccountId);
        return Ok(token);
    }

    [HttpPost("admin")]
    public IActionResult GetAdminJwt([FromBody] AdminAuthRequest? request)
    {
        if (request is null) return BadRequest("Request body is required");

        if (request.Password != passwordProvider.Password) return Unauthorized("Invalid admin password");
        var token = tokenService.GenerateAdminToken();
        return Ok(token);
    }
}