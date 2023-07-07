using FluentValidation;
using RapidPay.Api.Models;
using RapidPay.Api.Services.FeeService;
using RapidPay.Api.Services.Notify;
using RapidPay.DataAccess.Repository;

namespace RapidPay.Api.Validators
{
    public class MakePaymentModelValidator : AbstractValidator<MakePaymentModel>
    {
        public MakePaymentModelValidator()
        {
            RuleFor(x => x.CardNumber)
                .NotNull().NotEmpty().Length(15, 15)
                .WithMessage("The card number must have 15 digits");

            RuleFor(x => x.PaymentValue)
                .Must(x => x > 0)
                .WithMessage("The informed value must be greater than 0");
        }
    }

    public class MakePaymentModelManager : ICustomValidator<MakePaymentModel>
    {
        private readonly IValidator<MakePaymentModel> _validator;
        private readonly INotification _notification;
        private readonly ICardRepository _cardRepository;
        private readonly IFeeService _feeService;

        public MakePaymentModelManager(
            IValidator<MakePaymentModel> validator,
            INotification notification,
            ICardRepository cardRepository,
            IFeeService feeService)
        {
            _validator = validator;
            _notification = notification;
            _cardRepository = cardRepository;
            _feeService = feeService;
        }

        public async Task Manage(MakePaymentModel model)
        {
            if (model == null)
            {
                _notification.AddNotification("Request data shall be informed");
                return;
            }

            var result = await _validator.ValidateAsync(model);

            if (result == null)
                _notification.AddNotification("It was not possible to validate the data! Please contact the system admin");
            else if (!result.IsValid)
                foreach (var error in result.Errors)
                    _notification.AddNotification(error.ErrorMessage);
            else
            {
                string cardNumber = model.CardNumber.Trim().ToUpper();
                var card = await _cardRepository.GetByCardNumberAsyncNoTrack(cardNumber);

                if (card == null)
                    _notification.AddNotification("Card Number not found");
                else
                {
                    var fee = await _feeService.GetPaymentFee(model.PaymentValue);

                    var calculatedPaymentValue = fee.FeeValue + model.PaymentValue;

                    if (calculatedPaymentValue > card.Balance)
                        _notification.AddNotification("Card balance not enough for the payment");
                }
            }
        }
    }
}
