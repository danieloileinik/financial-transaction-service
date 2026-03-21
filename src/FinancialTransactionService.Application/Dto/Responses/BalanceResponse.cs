namespace FinancialTransactionService.Application.Dto.Responses;

public class BalanceResponse
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