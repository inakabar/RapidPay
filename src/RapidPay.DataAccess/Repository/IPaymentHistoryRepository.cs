using RapidPay.DataAccess.Entities;
using System.Threading.Tasks;

namespace RapidPay.DataAccess.Repository
{
    public interface IPaymentHistoryRepository
    {
        Task Create(PaymentHistory payment);
    }
}
