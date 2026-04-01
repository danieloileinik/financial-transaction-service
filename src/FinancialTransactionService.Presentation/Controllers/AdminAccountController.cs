using FinancialTransactionService.Application.Dto.Requests;
using FinancialTransactionService.Application.Dto.Responses;
using FinancialTransactionService.Application.UseCases.Accounts;
using FinancialTransactionService.Application.UseCases.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinancialTransactionService.Presentation.Controllers;

[Authorize(Policy = "AdminOnly")]
[ApiController]
[Route("api/admin/accounts")]
public class AdminAccountController(
    DeleteAccountHandler deleteAccountHandler,
    CreateAccountHandler createAccountHandler,
    LockAccountHandler lockAccountHandler,
    DepositMoneyHandler depositMoneyHandler,
    WithdrawMoneyHandler withdrawMoneyHandler,
    ViewTransactionsHandler viewTransactionsHandler) : ControllerBase
{
    [HttpPost("")]
    public async Task<ActionResult<AccountResponse>> CreateAccount([FromQuery] CancellationToken ct = default)
    {
        var accountId = await createAccountHandler.Execute(ct);
        return Ok(accountId.Value);
    }

    [HttpPost("{id:guid}/lock")]
    public async Task<IActionResult> LockAccount([FromHeader] Guid id, [FromQuery] CancellationToken ct = default)
    {
        var result = await lockAccountHandler.Lock(id, ct);
        return result.IsError ? ErrorHandler.Handle(result) : NoContent();
    }

    [HttpPost("{id:guid}/unlock")]
    public async Task<IActionResult> UnlockAccount(Guid id, [FromQuery] CancellationToken ct = default)
    {
        var result = await lockAccountHandler.Unlock(id, ct);
        return result.IsError ? ErrorHandler.Handle(result) : NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAccount(Guid id)
    {
        var result = await deleteAccountHandler.Execute(id);
        return result.IsError ? ErrorHandler.Handle(result) : NoContent();
    }

    [HttpPut("{id:guid}/balance")]
    public async Task<IActionResult> AdjustBalance(
        Guid id,
        [FromBody] MoneyOperationRequest request,
        [FromQuery] CancellationToken ct = default)
    {
        var result = request.Amount <= 0
            ? await withdrawMoneyHandler.Execute(id, new MoneyOperationRequest(Math.Abs(request.Amount)), ct)
            : await depositMoneyHandler.Execute(id, request, ct);

        return result.IsError ? ErrorHandler.Handle(result) : NoContent();
    }

    [HttpGet("{id:guid}/transactions")]
    public async Task<ActionResult<TransactionsResponse>> GetAccountTransactions(
        Guid id,
        [FromQuery] TransactionsRequest? request = null,
        CancellationToken ct = default)
    {
        var result = await viewTransactionsHandler.Execute(id, request, ct);
        return result.IsError ? ErrorHandler.Handle(result) : Ok(result.Value);
    }
}