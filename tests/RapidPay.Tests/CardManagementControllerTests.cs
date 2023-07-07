using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using RapidPay.Api.Controllers;
using RapidPay.Api.Models;
using RapidPay.Api.Services.Card;
using RapidPay.Api.Services.Notify;
using Xunit;

namespace RapidPay.Tests
{
    public class CardManagementControllerTests
    {
        private readonly Mock<ICardService> _cardServiceMock;
        private readonly Mock<INotification> _notificationMock;
        private readonly CardManagementController _controller;

        public CardManagementControllerTests()
        {
            _cardServiceMock = new Mock<ICardService>();
            _notificationMock = new Mock<INotification>();
            _controller = new CardManagementController(_cardServiceMock.Object, _notificationMock.Object);
        }

        [Fact]
        public async Task GetBalance_WithValidCardNumber_ShouldReturnOkResultWithData()
        {
            // Arrange
            var cardNumber = "123456789012345";
            var cardBalance = new CardBalanceModel { Balance = 100 };
            //_cardServiceMock.Setup(c => c.GetBalanceAsync( new GetBalanceModel(cardNumber))).ReturnsAsync(cardBalance);
            _cardServiceMock.Setup(c => c.GetBalanceAsync(It.IsAny<GetBalanceModel>())).ReturnsAsync(cardBalance);

            // Act
            var result = await _controller.GetBalance(cardNumber) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            var responseModel = Assert.IsType<BaseResponseModel<CardBalanceModel>>(result.Value);
            Assert.True(responseModel.Success);
            Assert.Null(responseModel.Errors);
            Assert.Equal(cardBalance, responseModel.Data);
        }

        [Fact]
        public async Task GetBalance_WithInvalidCardNumber_ShouldReturnBadRequestWithErrors()
        {
            // Arrange
            var cardNumber = "123";
            _notificationMock.Setup(n => n.IsThereNotification()).Returns(true);
            _notificationMock.Setup(n => n.GetNotifications()).Returns(new List<string> { "The card number must have 15 digits" });

            // Act
            var result = await _controller.GetBalance(cardNumber) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(400, result.StatusCode);
            var responseModel = Assert.IsType<BaseResponseModel<CardBalanceModel>>(result.Value);
            Assert.False(responseModel.Success);
            Assert.NotNull(responseModel.Errors);
            Assert.Equal("The card number must have 15 digits", Assert.Single(responseModel.Errors));
            Assert.Null(responseModel.Data);
        }

        [Fact]
        public async Task Create_WithValidModel_ShouldReturnOkResult()
        {
            // Arrange
            var model = new CreateCardModel { Name = "New Card", CardNumber = "123456789012345", Balance = 100 };

            // Act
            var result = await _controller.Create(model) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            var responseModel = Assert.IsType<BaseResponseModel<object>>(result.Value);
            Assert.True(responseModel.Success);
            Assert.Null(responseModel.Errors);
            Assert.Null(responseModel.Data);
        }
        
        [Fact]
        public async Task AddValue_WithValidModel_ShouldReturnOkResult()
        {
            // Arrange
            var model = new CardAddBalanceModel { CardNumber = "123456789012345", Value = 50 };

            // Act
            var result = await _controller.AddValue(model) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            var responseModel = Assert.IsType<BaseResponseModel<object>>(result.Value);
            Assert.True(responseModel.Success);
            Assert.Null(responseModel.Errors);
            Assert.Null(responseModel.Data);
        }

        [Fact]
        public async Task MakePayment_WithValidModel_ShouldReturnOkResult()
        {
            // Arrange
            var model = new MakePaymentModel { CardNumber = "123456789012345", PaymentValue = 50 };

            // Act
            var result = await _controller.MakePayment(model) as ObjectResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(200, result.StatusCode);
            var responseModel = Assert.IsType<BaseResponseModel<object>>(result.Value);
            Assert.True(responseModel.Success);
            Assert.Null(responseModel.Errors);
            Assert.Null(responseModel.Data);
        }
    }
}
