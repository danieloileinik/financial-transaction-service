namespace FinancialTransactionService.Application.Dto.Requests;

public record TransferRequest(Guid ReceiverId, MoneyOperationRequest Amount);