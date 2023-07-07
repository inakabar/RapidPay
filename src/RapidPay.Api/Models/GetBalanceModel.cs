namespace RapidPay.Api.Models
{
    public class GetBalanceModel
    {
        public GetBalanceModel(string cardNumber)
        {
            CardNumber = cardNumber?.Trim().ToUpper();
        }

        public string CardNumber { get; private set; }
    }
}
