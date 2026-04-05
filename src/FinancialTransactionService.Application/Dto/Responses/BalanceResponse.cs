namespace FinancialTransactionService.Application.Dto.Responses;

public record BalanceResponse
{
    public BalanceResponse(decimal balance)
    {
        Balance = balance;
    }

    public BalanceResponse()
    {
    }

    public decimal Balance { get; }
}