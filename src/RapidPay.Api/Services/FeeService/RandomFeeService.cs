using RapidPay.Api.Models;

namespace RapidPay.Api.Services.FeeService
{
    public class RandomFeeService : IFeeService
    {
        private decimal currentFee;
        private DateTime _lastUpdateOnFee;
        private Random random;

        public RandomFeeService()
        {
            random = new Random();
            currentFee = GenerateRandomValue();
            _lastUpdateOnFee = DateTime.Now;
        }

        // Async method, predicting it would became a third party service
        // Therefore the Interface may be kept, and change only the implementation class
        public async Task<GetPaymentFeeModel> GetPaymentFee(double payment)
        {
            var fee = await GetUniversalFeesExchange();

            return new GetPaymentFeeModel()
            {
                Fee = fee,
                FeeValue = payment * (double)fee
            };
        }

        private async Task<decimal> GetUniversalFeesExchange()
        {
            if (_lastUpdateOnFee.AddHours(1) < DateTime.Now)
            {
                GenerateNewFee();
                _lastUpdateOnFee = DateTime.Now;
            }

            await Task.FromResult(1);

            return currentFee;
        }

        private void GenerateNewFee()
        {
            
            currentFee *= GenerateRandomValue();

            currentFee = Math.Min(2, Math.Max(0, currentFee));
        }

        private decimal GenerateRandomValue()
        {
            return Convert.ToDecimal(random.NextDouble() * 2);
        }
    }
}
