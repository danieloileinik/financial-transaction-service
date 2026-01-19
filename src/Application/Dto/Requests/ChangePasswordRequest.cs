namespace Application.Dto.Requests;

public record ChangePasswordRequest(string OldPassword, string NewPassword);