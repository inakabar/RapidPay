using RapidPay.DataAccess.Entities;

namespace RapidPay.Api.Models
{
    public class CardModel
    {
        public string Name { get; set; }
        public string CardNumber { get; set; }
        public Double Balance { get; set; }
        public bool Active { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime LastUpdateDate { get; set; }

        public static CardModel FromEntity(Card card)
        {
            if (card == null) return null;

            return new CardModel()
            {
                Name = card.Name,
                CardNumber = card.Number,
                Balance = card.Balance,
                Active = card.Active,
                CreateDate = card.CreateDate,
                LastUpdateDate = card.LastUpdateDate
            };
        }

        public Card ToModel()
        {
            return new DataAccess.Entities.Card
            {
                Name = this.Name.Trim(),
                Number = this.CardNumber?.Trim().ToUpper(),
                Balance = this.Balance,
                CreateDate = DateTime.Now,
                LastUpdateDate = DateTime.Now,
                Active = true
            };
        }
    }
}
