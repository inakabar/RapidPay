using RapidPay.DataAccess.Entities;

namespace RapidPay.Api.Models
{
    public class CardBalanceModel
    {
        public Double Balance { get; set; }

        public static CardBalanceModel FromEntity(Card card)
        {
            if(card == null) return null;

            return new CardBalanceModel()
            {
                Balance = card.Balance
            };
        }
    }
}
