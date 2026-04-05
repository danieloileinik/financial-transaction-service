namespace FinancialTransactionService.Application.Dto.Requests;

public readonly record struct RegisterRequest(
    string Pin,
    string Password);