using RapidPay.DataAccess.Data;
using RapidPay.DataAccess.Entities;
using System;
using System.Threading.Tasks;

namespace RapidPay.DataAccess.Repository
{
    public class PaymentHistoryRepository : IPaymentHistoryRepository
    {
        private readonly IDataAccessLayer _dataAccessLayer;
        public PaymentHistoryRepository(IDataAccessLayer dataAccessLayer)
        {
            _dataAccessLayer = dataAccessLayer ?? throw new ArgumentNullException();
        }

        public async Task Create(PaymentHistory payment)
        {
            await _dataAccessLayer.PaymentHistories.AddAsync(payment);
        }

        public async void Dispose()
        {
            await _dataAccessLayer.SaveChangesAsync();
        }
    }
}
