using FinancialTransactionService.Application.Abstractions.Security;
using FinancialTransactionService.Application.Dto.Requests;
using FinancialTransactionService.Application.UseCases.Accounts;
using Microsoft.AspNetCore.Mvc;

namespace FinancialTransactionService.Presentation.Controllers;

[ApiController]
[Route("/api/auth")]
public class AuthController(
    ISystemPasswordProvider passwordProvider,
    ITokenService tokenService,
    AccessAccountHandler accessAccountHandler) : ControllerBase
{
    [HttpPost("user/atm")]
    public async Task<IActionResult> GetUserJwt(
        [FromBody] UserAuthRequest.UserAtmAuthRequest request,
        [FromQuery] CancellationToken ct = default)
    {
        var result = await accessAccountHandler.GetFromAtmAsync(request, ct);
        if (result.IsError) return ErrorHandler.Handle(result);

        var token = tokenService.GenerateUserToken(request.AccountId);
        return Ok(token);
    }

    [HttpPost("user/online")]
    public async Task<IActionResult> GetUserJwt(
        [FromBody] UserAuthRequest.UserOnlineAuthRequest request,
        [FromQuery] CancellationToken ct = default)
    {
        var result = await accessAccountHandler.GetOnline(request, ct);
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