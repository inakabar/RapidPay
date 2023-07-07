using Microsoft.EntityFrameworkCore;
using RapidPay.DataAccess.Data;
using RapidPay.DataAccess.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RapidPay.DataAccess.Repository
{
    public class CardRepository : ICardRepository
    {
        private readonly IDataAccessLayer _dataAccessLayer;
        public CardRepository(IDataAccessLayer dataAccessLayer)
        {
            _dataAccessLayer = dataAccessLayer ?? throw new ArgumentNullException();
        }

        public async Task CreateAsync(Card card)
        {
            await _dataAccessLayer.Cards.AddAsync(card);

            await _dataAccessLayer.SaveChangesAsync();
        }

        public async Task UpdateAsync(Card card, double value)
        {
            card.Balance += value;
            card.LastUpdateDate = DateTime.UtcNow;

            await _dataAccessLayer.SaveChangesAsync();
        }

        public async Task<bool> IsCardRegistered(string cardNumber)
        {
            var card = await _dataAccessLayer.Cards
                        .AsNoTracking().FirstOrDefaultAsync(card => card.Number == cardNumber);
            return card != null;
        }

        public async Task<bool> IsCardRegisteredAndActive(string cardNumber)
        {
            var card = await _dataAccessLayer.Cards
                        .AsNoTracking().FirstOrDefaultAsync(card => card.Number == cardNumber && card.Active);
            return card != null;
        }

        public async Task<Card?> GetByCardNumberAsyncWithTrack(string cardNumber)
        {
            return await _dataAccessLayer.Cards
                        .FirstOrDefaultAsync(card => card.Number == cardNumber && card.Active);
        }
        
        public async Task<Card?> GetByCardNumberAsyncNoTrack(string cardNumber)
        {
            return await _dataAccessLayer.Cards
                        .AsNoTracking().FirstOrDefaultAsync(card => card.Number == cardNumber && card.Active);
        }

        public async Task<List<Card>> ListAsync()
        {
            return await _dataAccessLayer.Cards
                .Where(card => card.Active)
                .Select(card => card)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
