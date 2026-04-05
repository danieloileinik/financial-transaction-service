namespace FinancialTransactionService.Application.Dto.Requests;

public record TransactionRequest(DateTimeOffset From, DateTimeOffset To);