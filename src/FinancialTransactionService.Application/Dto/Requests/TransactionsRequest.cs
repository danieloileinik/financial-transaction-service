namespace FinancialTransactionService.Application.Dto.Requests;

public record TransactionsRequest(DateTimeOffset From, DateTimeOffset To);
// public readonly record struct TransactionsRequest(DateTimeOffset From, DateTimeOffset To);