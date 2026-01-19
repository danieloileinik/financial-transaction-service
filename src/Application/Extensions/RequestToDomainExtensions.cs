using Application.Dto.Requests;
using Domain.ValueObjects;
using ErrorOr;

namespace Application.Extensions;

public static class RequestToDomainExtensions
{
    public static ErrorOr<Money> ToDomain(this MoneyOperationRequest request)
    {
        return Money.Create(request.Amount);
    }

    public static ErrorOr<PinCode> ToDomain(this SetPinCodeRequest request)
    {
        return PinCode.Create(request.Pin);
    }

    public static ErrorOr<PinCode> GetPinCode(this UserAuthRequest.UserAtmAuthRequest request)
    {
        return PinCode.Create(request.Pin);
    }
}