namespace FinancialTransactionService.Application.Dto.Requests;

public record ChangePasswordRequest(string OldPassword, string NewPassword);
// public readonly record struct ChangePasswordRequest(string OldPassword, string NewPassword);