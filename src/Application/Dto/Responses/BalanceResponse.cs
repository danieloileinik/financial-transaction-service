namespace Application.Dto.Responses;

public class BalanceResponse
{
    public decimal Balance { get; }

    public BalanceResponse(decimal balance)
    {
        Balance = balance;
    }

    public BalanceResponse()
    {
    }
}