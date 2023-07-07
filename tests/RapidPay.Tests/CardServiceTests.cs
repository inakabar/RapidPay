using Castle.Core.Logging;
using Coderful.EntityFramework.Testing.Mock;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestPlatform.CommunicationUtilities;
using Moq;
using Newtonsoft.Json.Linq;
using RapidPay.Api.Models;
using RapidPay.Api.Services.Card;
using RapidPay.Api.Services.FeeService;
using RapidPay.Api.Services.Notify;
using RapidPay.Api.Validators;
using RapidPay.DataAccess.Data;
using RapidPay.DataAccess.Entities;
using RapidPay.DataAccess.Repository;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using System.Reflection.Metadata;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace RapidPay.Tests
{
    public class CardServiceTests
    {
        private readonly Mock<INotification> _notificationMock;
        private readonly Mock<IFeeService> _feeServiceMock;
        private readonly Mock<ICardRepository> _cardRepositoryMock;
        private readonly Mock<IPaymentHistoryRepository> _paymentHistoryRepositoryMock;

        private readonly Mock<ICustomValidator<CreateCardModel>> _cardModelValidatorMock;
        private readonly Mock<ICustomValidator<CardAddBalanceModel>> _cardAddBalanceModelValidatorMock;
        private readonly Mock<ICustomValidator<MakePaymentModel>> _makePaymentModelValidatorMock;
        private readonly Mock<ICustomValidator<GetBalanceModel>> _getBalanceModelValidatorMock;

        private readonly CardService _cardService;

        public CardServiceTests()
        {
            _notificationMock = new Mock<INotification>();
            _feeServiceMock = new Mock<IFeeService>();
            _cardRepositoryMock = new Mock<ICardRepository>();
            _paymentHistoryRepositoryMock = new Mock<IPaymentHistoryRepository>();

            _cardModelValidatorMock = new Mock<ICustomValidator<CreateCardModel>>();
            _cardAddBalanceModelValidatorMock = new Mock<ICustomValidator<CardAddBalanceModel>>();
            _makePaymentModelValidatorMock = new Mock<ICustomValidator<MakePaymentModel>>();
            _getBalanceModelValidatorMock = new Mock<ICustomValidator<GetBalanceModel>>();


            _cardService = new CardService(
                _notificationMock.Object,
                _feeServiceMock.Object,
                _cardRepositoryMock.Object,
                _paymentHistoryRepositoryMock.Object,
                _cardModelValidatorMock.Object,
                _cardAddBalanceModelValidatorMock.Object,
                _makePaymentModelValidatorMock.Object,
                _getBalanceModelValidatorMock.Object
                );
        }

        [Fact]
        public async Task ListAsync_ShouldReturnListOfCardModels()
        {
            // Arrange
            var cards = new List<DataAccess.Entities.Card>
            {
                new DataAccess.Entities.Card
                {
                    Name = "Card 1",
                    Number = "123456789012345",
                    Balance = 100.00,
                    CreateDate = DateTime.Now,
                    LastUpdateDate = DateTime.Now
                },
                new DataAccess.Entities.Card
                {
                    Name = "Card 2",
                    Number = "987654321098765",
                    Balance = 200.00,
                    CreateDate = DateTime.Now,
                    LastUpdateDate = DateTime.Now
                }
            };

            _cardRepositoryMock.Setup(x => x.ListAsync())
                .ReturnsAsync(cards) ;

            // Act
            var result = await _cardService.ListAsync();

            var response = result.ToList();
            // Assert
            Assert.NotNull(result);
            Assert.Equal(cards.Count, response.Count) ;
            Assert.Equal(cards[0].Name, response[0].Name);
            Assert.Equal(cards[1].Number, response[1].CardNumber);
            Assert.Equal(cards[0].Balance, response[0].Balance);
            Assert.Equal(cards[1].Active, response[1].Active);
            Assert.Equal(cards[0].CreateDate, response[0].CreateDate);
            Assert.Equal(cards[1].LastUpdateDate, response[1].LastUpdateDate);
        }

        [Fact]
        public async Task ListAsync_ReturnsListOfCards()
        {
            // Arrange
            var cards = new List<Card>
            {
                new Card { Number = "123456789012345", Name = "John Doe", Balance = 100 },
                new Card { Number = "543210987654321", Name = "Jane Smith", Balance = 200 }
            };

            _cardRepositoryMock.Setup(mock => mock.ListAsync()).ReturnsAsync(cards);

            // Act
            var result = await _cardService.ListAsync();

            // Assert
            Assert.Equal(cards.Count, result.Count());
            foreach (var card in result)
            {
                Assert.IsType<CardModel>(card);
            }
        }

        [Fact]
        public async Task GetBalanceAsync_ValidCardNumber_ReturnsCardBalance()
        {
            // Arrange
            var cardNumber = "123456789012345";
            var card = new Card { Number = cardNumber, Name = "John Doe", Balance = 100 };

            _cardRepositoryMock.Setup(mock => mock.GetByCardNumberAsyncNoTrack(cardNumber)).ReturnsAsync(card);

            // Act
            var result = await _cardService.GetBalanceAsync(new GetBalanceModel(cardNumber));

            // Assert
            Assert.NotNull(result);
            Assert.Equal(card.Balance, result.Balance);
        }

        [Fact]
        public async Task GetBalanceAsync_InvalidCardNumber_ReturnsNull()
        {
            // Arrange
            var cardNumber = "12345"; // Invalid card number length

            _notificationMock.Setup(mock => mock.IsThereNotification()).Returns(true);

            // Act
            var result = await _cardService.GetBalanceAsync(new GetBalanceModel(cardNumber));

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task CreateAsync_ValidCardModel_CreatesNewCard()
        {
            // Arrange
            var model = new CreateCardModel { CardNumber = "123456789012345", Name = "John Doe", Balance = 100 };

            _cardRepositoryMock.Setup(mock => mock.GetByCardNumberAsyncNoTrack(model.CardNumber)).ReturnsAsync((Card)null);

            // Act
            await _cardService.CreateAsync(model);

            // Assert
            _cardRepositoryMock.Verify(mock => mock.CreateAsync(It.IsAny<DataAccess.Entities.Card>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_NullCardModel_DoesNotCreateNewCard()
        {
            // Arrange
            CreateCardModel model = null;

            _notificationMock.Setup(mock => mock.IsThereNotification()).Returns(true);

            // Act
            await _cardService.CreateAsync(model);

            // Assert
            _cardRepositoryMock.Verify(mock => mock.CreateAsync(It.IsAny<DataAccess.Entities.Card>()), Times.Never);
        }

        [Fact]
        public async Task CreateAsync_InvalidCardNumber_AddsNotification()
        {
            // Arrange
            var model = new CreateCardModel { CardNumber = "12345", Name = "John Doe", Balance = 100 }; // Invalid card number length

            _notificationMock.Setup(mock => mock.IsThereNotification()).Returns(true);

            // Act
            await _cardService.CreateAsync(model);

            // Assert
            _cardRepositoryMock.Verify(mock => mock.CreateAsync(It.IsAny<DataAccess.Entities.Card>()), Times.Never);
        }

        [Fact]
        public async Task AddValueAsync_ValidCardNumber_AddsValueToCardBalance()
        {
            // Arrange
            var cardNumber = "123456789012345";
            var card = new Card { Number = cardNumber, Name = "John Doe", Balance = 100 };
            var model = new CardAddBalanceModel { CardNumber = cardNumber, Value = 50 };

            _cardRepositoryMock.Setup(mock => mock.GetByCardNumberAsyncWithTrack(cardNumber)).ReturnsAsync(card);
            _cardRepositoryMock
                .Setup(mock => mock.UpdateAsync(It.IsAny<Card>(), It.IsAny<double>()))
                .Callback(() => card.Balance += model.Value);

            // Act
            await _cardService.AddValueAsync(model);

            // Assert
            Assert.Equal(150, card.Balance);
            _cardRepositoryMock.Verify(mock => mock.GetByCardNumberAsyncWithTrack(cardNumber), Times.Once);
        }

        [Fact]
        public async Task AddValueAsync_InvalidCardNumber_AddsNotification()
        {
            // Arrange
            var cardNumber = "12345"; // Invalid card number length
            var model = new CardAddBalanceModel { CardNumber = cardNumber, Value = 50 };

            _notificationMock.Setup(mock => mock.IsThereNotification()).Returns(true);

            // Act
            await _cardService.AddValueAsync(model);

            // Assert
            _cardRepositoryMock.Verify(mock => mock.UpdateAsync(It.IsAny<DataAccess.Entities.Card>(), It.IsAny<double>()), Times.Never);
        }

        [Fact]
        public async Task MakePaymentAsync_ValidPayment_MakesPaymentAndUpdateBalance()
        {
            GetPaymentFeeModel fee = new GetPaymentFeeModel()
            {
                FeeValue = 50,
                Fee = 2
            };

            // Arrange
            var model = new MakePaymentModel
            {
                CardNumber = "123456789012345",
                PaymentValue = 50
            };

            var card = new Card { Number = model.CardNumber, Name = "John Doe", Balance = 200 };

            _cardRepositoryMock.Setup(mock => mock.GetByCardNumberAsyncWithTrack(model.CardNumber)).ReturnsAsync(card);
            _feeServiceMock.Setup(mock => mock.GetPaymentFee(model.PaymentValue)).ReturnsAsync(fee);
            _notificationMock.Setup(mock => mock.GetNotifications()).Returns(new List<string>());

            _cardRepositoryMock
                .Setup(mock => mock.UpdateAsync(It.IsAny<Card>(), It.IsAny<double>()))
                .Callback(() => card.Balance = card.Balance - (model.PaymentValue + (model.PaymentValue * (double)fee.Fee)));

            // Act
            await _cardService.MakePaymentAsync(model);

            // Assert
            Assert.Equal(50, card.Balance);
            _cardRepositoryMock.Verify(mock => mock.GetByCardNumberAsyncWithTrack(model.CardNumber), Times.Once);
            _paymentHistoryRepositoryMock.Verify(mock => mock.Create(It.IsAny<PaymentHistory>()), Times.Once);
        }
    }
}

