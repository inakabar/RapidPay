﻿using RapidPay.Api.Models;
using FluentValidation;
using RapidPay.Api.Services.Notify;
using RapidPay.DataAccess.Repository;

namespace RapidPay.Api.Validators
{
    public class CreateCardModelValidator : FluentValidation.AbstractValidator<CreateCardModel>
    {
        public CreateCardModelValidator() 
        {
            RuleFor(x => x.Name)
                .NotNull().NotEmpty()
                .WithMessage("The client name must be informed");

            RuleFor(x => x.CardNumber)
                .NotNull().NotEmpty().Length(15, 15)
                .WithMessage("The card number must have 15 digits");

            RuleFor(x => x.Balance)
                .Must(x => x > 0)
                .WithMessage("The initial Balance must be greater than 0");
        }
    }

    public class CreateCardModelManager : ICustomValidator<CreateCardModel>
    {
        private readonly IValidator<CreateCardModel> _validator;
        private readonly INotification _notification;
        private readonly ICardRepository _cardRepository;

        public CreateCardModelManager(IValidator<CreateCardModel> validator, INotification notification, ICardRepository cardRepository)
        {
            _validator = validator;
            _notification = notification;
            _cardRepository = cardRepository;
        }

        public async Task Manage(CreateCardModel model)
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
                foreach(var error in result.Errors)
                    _notification.AddNotification(error.ErrorMessage);
            else
            {
                string cardNumber = model.CardNumber.Trim().ToUpper();
                var isCardRegistered = await _cardRepository.IsCardRegistered(cardNumber);

                if (isCardRegistered)
                    _notification.AddNotification($"Card Number Already Registered");
            }
        }
    }
}
