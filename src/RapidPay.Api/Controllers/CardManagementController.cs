using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RapidPay.Api.Models;
using RapidPay.Api.Services.Card;
using RapidPay.Api.Services.Notify;
using RapidPay.Api.Validators;
using System.ComponentModel.DataAnnotations;

namespace RapidPay.Api.Controllers
{
    [Route("api/[controller]")]
    public class CardManagementController : BaseController
    {
        private readonly INotification _notification;
        private readonly ICardService _cardService;

        public CardManagementController(
            ICardService cardService, 
            INotification notification) 
            : base(notification)
        {
            _notification = notification;
            _cardService = cardService;
        }

        [HttpGet("")]
        [Authorize("Read")]
        [ProducesResponseType(400, Type = typeof(BaseResponseModel<CardBalanceModel>))]
        [ProducesResponseType(500, Type = typeof(BaseResponseModel<object>))]
        [ProducesResponseType(200, Type = typeof(BaseResponseModel<CardBalanceModel>))]
        public async Task<IActionResult> GetBalance([FromQuery]string cardNumber)
        {
            var cardBalance = await _cardService.GetBalanceAsync(new GetBalanceModel(cardNumber));

            return CustomReturn<CardBalanceModel>(cardBalance);
        }

        [HttpPost("")]
        [Authorize("Write")]
        [ProducesResponseType(400, Type = typeof(BaseResponseModel<object>))]
        [ProducesResponseType(500, Type = typeof(BaseResponseModel<object>))]
        [ProducesResponseType(200, Type = typeof(BaseResponseModel<object>))]
        public async Task<IActionResult> Create([FromBody]CreateCardModel model)
        {
            await _cardService.CreateAsync(model);

            return CustomReturn<object>();
        }

        [HttpPut("")]
        [Authorize("Write")]
        [ProducesResponseType(400, Type = typeof(BaseResponseModel<object>))]
        [ProducesResponseType(500, Type = typeof(BaseResponseModel<object>))]
        [ProducesResponseType(200, Type = typeof(BaseResponseModel<object>))]
        public async Task<IActionResult> AddValue([FromBody] CardAddBalanceModel model)
        {
            await _cardService.AddValueAsync(model);

            return CustomReturn<object>();
        }

        [HttpPut("MakePayment")]
        [Authorize("Write")]
        [ProducesResponseType(400, Type = typeof(BaseResponseModel<object>))]
        [ProducesResponseType(500, Type = typeof(BaseResponseModel<object>))]
        [ProducesResponseType(200, Type = typeof(BaseResponseModel<object>))]
        public async Task<IActionResult> MakePayment([FromBody] MakePaymentModel model)
        {
            await _cardService.MakePaymentAsync(model);

            return CustomReturn<object>();
        }
    }
}
