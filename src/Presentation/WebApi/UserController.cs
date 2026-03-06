using System.Diagnostics;
using System.Security.Claims;
using Application.Dto.Requests;
using Application.Dto.Responses;
using Application.UseCases.Accounts;
using Application.UseCases.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.WebApi;

[Authorize]
[ApiController]
[Route("/api/account")]
public class UserController(
    ViewBalanceHandler balanceHandler,
    WithdrawMoneyHandler withdrawMoneyHandler,
    DepositMoneyHandler depositMoneyHandler,
    ViewTransactionsHandler viewTransactionsHandler,
    TransferMoneyHandler transferMoneyHandler,
    SetPinHandler setPinHandler,
    SetPasswordHandler setPasswordHandler,
    ChangePasswordHandler changePasswordHandler,
    DeleteAccountHandler deleteAccountHandler) : ControllerBase
{
    private Guid AccountId
    {
        get
        {
            var claim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (claim is null || !Guid.TryParse(claim.Value, out var accountId)) throw new UnreachableException();
            return accountId;
        }
    }

    [HttpGet("balance")]
    public async Task<ActionResult<BalanceResponse>> GetBalance([FromQuery] CancellationToken ct = default)
    {
        var result = await balanceHandler.Execute(AccountId, ct);
        return result.IsError ? ErrorHandler.Handle(result) : Ok(result.Value);
    }

    [HttpGet("transactions")]
    public async Task<ActionResult<TransactionsResponse>> GetTransactions(
        [FromQuery] TransactionsRequest? request = null,
        CancellationToken ct = default)
    {
        var result = await viewTransactionsHandler.Execute(AccountId, request, ct);
        return result.IsError ? ErrorHandler.Handle(result) : Ok(result.Value);
    }

    [HttpDelete("")]
    public async Task<IActionResult> DeleteAccount()
    {
        var result = await deleteAccountHandler.Execute(AccountId);
        return result.IsError ? ErrorHandler.Handle(result) : NoContent();
    }

    [HttpPut("pin")]
    public async Task<ActionResult> SetPin(
        [FromBody] SetPinCodeRequest request,
        [FromQuery] CancellationToken ct = default)
    {
        var result = await setPinHandler.Execute(AccountId, request, ct);
        return result.IsError ? ErrorHandler.Handle(result) : NoContent();
    }

    [HttpPut("password/set")]
    public async Task<ActionResult> SetPassword(
        [FromBody] SetPasswordRequest request,
        [FromQuery] CancellationToken ct = default)
    {
        var result = await setPasswordHandler.Execute(AccountId, request, ct);
        return result.IsError ? ErrorHandler.Handle(result) : NoContent();
    }

    [HttpPut("password/change")]
    public async Task<ActionResult> ChangePassword(
        [FromBody] ChangePasswordRequest request,
        [FromQuery] CancellationToken ct = default)
    {
        var result = await changePasswordHandler.Execute(AccountId, request, ct);
        return result.IsError ? ErrorHandler.Handle(result) : NoContent();
    }

    [HttpPost("deposit/atm")]
    public async Task<ActionResult> Deposit(
        [FromBody] MoneyOperationRequest request,
        [FromQuery] CancellationToken ct = default)
    {
        var result = await depositMoneyHandler.Execute(AccountId, request, ct);
        return result.IsError ? ErrorHandler.Handle(result) : NoContent();
    }

    [HttpPost("withdraw/atm")]
    public async Task<ActionResult> Withdraw(
        [FromBody] MoneyOperationRequest request,
        [FromQuery] CancellationToken ct = default)
    {
        var result = await withdrawMoneyHandler.Execute(AccountId, request, ct);
        return result.IsError ? ErrorHandler.Handle(result) : NoContent();
    }

    [HttpPost("transfer")]
    public async Task<ActionResult> Transfer(
        [FromBody] TransferRequest request,
        [FromQuery] CancellationToken ct = default)
    {
        var result = await transferMoneyHandler.Execute(AccountId, request, ct);
        return result.IsError ? ErrorHandler.Handle(result) : NoContent();
    }
}