namespace FinancialTransactionService.Application.Dto.Requests;

public record TransferRequest(Guid ReceiverId, MoneyOperationRequest Amount);
// public readonly record struct TransferRequest(Guid ReceiverId, MoneyOperationRequest Amount);