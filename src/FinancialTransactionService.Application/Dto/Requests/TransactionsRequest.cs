namespace FinancialTransactionService.Application.Dto.Requests;

public record TransactionsRequest(DateTimeOffset From, DateTimeOffset To);