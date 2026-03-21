using ErrorOr;
using FinancialTransactionService.Application.Abstractions.Persistence;
using FinancialTransactionService.Application.Dto.Requests;
using FinancialTransactionService.Application.Extensions;
using FinancialTransactionService.Domain.Errors;
using FinancialTransactionService.Domain.Models;

namespace FinancialTransactionService.Application.UseCases.Transactions;

public class TransferMoneyHandler(
    IAccountRepository accountRepository,
    ITransactionsRepository transactionsRepository,
    IUnitOfWork unitOfWork)
{
    public async Task<ErrorOr<Success>> Execute(Guid sender, TransferRequest request, CancellationToken ct = default)
    {
        var senderAccount = await accountRepository.GetByIdAsync(sender, ct);
        if (senderAccount is null) return AccountErrors.NotFound(sender);

        var receiver = request.ReceiverId;
        var receiverAccount = await accountRepository.GetByIdAsync(receiver, ct);
        if (receiverAccount is null) return AccountErrors.NotFound(receiver);

        var money = request.Amount.ToDomain();
        if (money.IsError) return money.FirstError;
        var amount = money.Value;

        var result = senderAccount.Withdraw(amount);
        if (result.IsError) return result.FirstError;
        transactionsRepository.Add(new WithdrawTransaction(sender, amount, DateTime.Now));

        result = receiverAccount.Deposit(amount);
        if (result.IsError) return result.FirstError;
        transactionsRepository.Add(new DepositTransaction(receiver, amount, DateTime.Now));

        await unitOfWork.SaveChangesAsync(ct);
        return Result.Success;
    }
}