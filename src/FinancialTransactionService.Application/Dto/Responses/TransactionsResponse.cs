namespace FinancialTransactionService.Application.Dto.Responses;

public record TransactionsResponse(IReadOnlyList<TransactionsResponse.TransactionResponse> Transactions)
{
    public record TransactionResponse(string Type, decimal Amount, DateTimeOffset Timestamp);
}