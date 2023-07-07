using RapidPay.Api.Models;

namespace RapidPay.Api.Services.FeeService
{
    public interface IFeeService
    {
        Task<GetPaymentFeeModel> GetPaymentFee(double payment);
    }
}
