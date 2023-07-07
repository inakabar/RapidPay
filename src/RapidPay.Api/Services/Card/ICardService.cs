using RapidPay.Api.Models;

namespace RapidPay.Api.Services.Card
{
    public interface ICardService
    {
        Task<IEnumerable<CardModel>> ListAsync();
        Task<CardBalanceModel> GetBalanceAsync(GetBalanceModel model);
        Task CreateAsync(CreateCardModel model);
        Task AddValueAsync(CardAddBalanceModel model);

        Task MakePaymentAsync(MakePaymentModel model);
    }
}
