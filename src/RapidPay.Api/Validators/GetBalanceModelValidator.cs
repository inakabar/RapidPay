using FluentValidation;
using RapidPay.Api.Models;
using RapidPay.Api.Services.Notify;
using RapidPay.DataAccess.Repository;

namespace RapidPay.Api.Validators
{
    public class GetBalanceModelValidator : AbstractValidator<GetBalanceModel>
    {
        public GetBalanceModelValidator()
        {
            RuleFor(x => x.CardNumber)
                .NotNull().NotEmpty().Length(15, 15)
                .WithMessage("The card number must have 15 digits");
        }
    }

    public class GetBalanceModelManager : ICustomValidator<GetBalanceModel>
    {
        private readonly IValidator<GetBalanceModel> _validator;
        private readonly INotification _notification;
        private readonly ICardRepository _cardRepository;

        public GetBalanceModelManager(IValidator<GetBalanceModel> validator, INotification notification, ICardRepository cardRepository)
        {
            _validator = validator;
            _notification = notification;
            _cardRepository = cardRepository;
        }

        public async Task Manage(GetBalanceModel model)
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
                var isCardRegistered = await _cardRepository.IsCardRegisteredAndActive(cardNumber);

                if (!isCardRegistered)
                    _notification.AddNotification("Card Number not found");
            }
        }
    }
}
