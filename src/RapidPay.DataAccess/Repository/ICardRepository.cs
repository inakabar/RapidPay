using RapidPay.DataAccess.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RapidPay.DataAccess.Repository
{
    public interface ICardRepository
    {
        Task<List<Card>> ListAsync();
        Task<bool> IsCardRegisteredAndActive(string cardNumber);
        Task<bool> IsCardRegistered(string cardNumber);
        Task<Card?> GetByCardNumberAsyncWithTrack(string cardNumber);
        Task<Card?> GetByCardNumberAsyncNoTrack(string cardNumber);
        Task CreateAsync(Card card);
        Task UpdateAsync(Card card, double value);
    }
}
