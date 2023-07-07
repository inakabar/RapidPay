using RapidPay.Api.Models;
using RapidPay.Api.Services.FeeService;
using RapidPay.Api.Services.Notify;
using RapidPay.Api.Validators;
using RapidPay.DataAccess.Entities;
using RapidPay.DataAccess.Repository;

namespace RapidPay.Api.Services.Card
{
    public class CardService : ICardService
    {
        private readonly INotification _notification;
        private readonly IFeeService _feeService;
        private readonly ICardRepository _cardRepository;
        private readonly IPaymentHistoryRepository _paymentHistoryRepository;
        private readonly ICustomValidator<CreateCardModel> _cardModelValidator;
        private readonly ICustomValidator<CardAddBalanceModel> _cardAddBalanceModelValidator;
        private readonly ICustomValidator<MakePaymentModel> _makePaymentModelValidator;
        private readonly ICustomValidator<GetBalanceModel> _getBalanceModelValidator;

        public CardService(
            INotification notification,
            IFeeService feeService,
            ICardRepository cardRepository,
            IPaymentHistoryRepository paymentHistoryRepository,
            ICustomValidator<CreateCardModel> cardModelValidator,
            ICustomValidator<CardAddBalanceModel> cardAddBalanceModelValidator,
            ICustomValidator<MakePaymentModel> makePaymentModelValidator,
            ICustomValidator<GetBalanceModel> getBalanceModelValidator)
        {
            _notification = notification ?? throw new ArgumentNullException();
            _feeService = feeService ?? throw new ArgumentNullException();
            _cardRepository = cardRepository ?? throw new ArgumentNullException();
            _paymentHistoryRepository = paymentHistoryRepository ?? throw new ArgumentNullException();
            _cardModelValidator = cardModelValidator;
            _cardAddBalanceModelValidator = cardAddBalanceModelValidator;
            _makePaymentModelValidator = makePaymentModelValidator;
            _getBalanceModelValidator = getBalanceModelValidator;
        }

        public async Task<IEnumerable<CardModel>> ListAsync()
        {
            var cards = await _cardRepository.ListAsync();

            return cards.Select(card => CardModel.FromEntity(card));
        }

        public async Task<CardBalanceModel> GetBalanceAsync(GetBalanceModel model)
        {
            await _getBalanceModelValidator.Manage(model);

            if (_notification.IsThereNotification())
                return null;

            var card = await _cardRepository.GetByCardNumberAsyncNoTrack(model.CardNumber);

            _notification.AddMessage($"Balance for card number({model.CardNumber}): ${card.Balance}");
            return CardBalanceModel.FromEntity(card);
        }

        public async Task CreateAsync(CreateCardModel model)
        {
            await _cardModelValidator.Manage(model);

            if (_notification.IsThereNotification())
                return;
            
            var newCard = model.ToModel();

            await _cardRepository.CreateAsync(newCard);
            _notification.AddMessage($"Registered card number ({model.CardNumber})");
        }

        public async Task AddValueAsync(CardAddBalanceModel model)
        {
            await _cardAddBalanceModelValidator.Manage(model);

            if (_notification.IsThereNotification())
                return;

            string cardNumber = model.CardNumber.Trim().ToUpper();
            var card = await _cardRepository.GetByCardNumberAsyncWithTrack(cardNumber);

            double currentBalance = card.Balance;
            await _cardRepository.UpdateAsync(card, model.Value);
            _notification.AddMessage($"Updated balance for card number ({model.CardNumber}) from ${currentBalance} to ${card.Balance}");
        }

        public async Task MakePaymentAsync(MakePaymentModel model)
        {
            var fee = await _feeService.GetPaymentFee(model?.PaymentValue ?? 0);
            string cardNumber = model.CardNumber.Trim().ToUpper();

            await _makePaymentModelValidator.Manage(model);

            await RegisterPaymentHistoryAsync(model, fee.Fee);

            if (_notification.IsThereNotification())
                return;

            var card = await _cardRepository.GetByCardNumberAsyncWithTrack(cardNumber);
            
            var paymentValue = model.PaymentValue + fee.FeeValue;

            double currentBalance = card.Balance;
            await _cardRepository.UpdateAsync(card, -(paymentValue));
            _notification.AddMessage($"Updated balance for card number ({model.CardNumber}) from ${currentBalance} to ${card.Balance}");
        }

        private async Task RegisterPaymentHistoryAsync(MakePaymentModel model, decimal fee, double? currentBalance = null)
        {
            var payment = new PaymentHistory
            {
                PaymentId = Guid.NewGuid(),
                CardNumber = model?.CardNumber ?? "NOTINFORMED",
                Value = model?.PaymentValue ?? 0.0,
                Date = DateTime.Now,
                Success = !_notification.IsThereNotification(),
                Message = _notification.GetNotifications().Count > 0
                    ? string.Join(",", _notification.GetNotifications())
                    : "",
                Fee = (double)fee,
                CurrentlyBalance = currentBalance
            };

            await _paymentHistoryRepository.Create(payment);
        }
    }
}
