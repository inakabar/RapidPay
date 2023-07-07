using RapidPay.DataAccess.Entities;

namespace RapidPay.Api.Models
{
    public class CreateCardModel
    {
        public string Name { get; set; }
        public string CardNumber { get; set; }
        public Double Balance { get; set; }

        public static CreateCardModel FromEntity(Card card)
        {
            if (card == null) return null;

            return new CreateCardModel()
            {
                Name = card.Name,
                CardNumber = card.Number,
                Balance = card.Balance
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
