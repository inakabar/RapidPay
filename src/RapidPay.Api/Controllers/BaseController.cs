using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RapidPay.Api.Models;
using RapidPay.Api.Services.Notify;

namespace RapidPay.Api.Controllers
{
    [ApiController]
    [Authorize]
    public class BaseController : ControllerBase
    {
        private readonly INotification _notification;

        public BaseController(INotification notification)
        {
            _notification = notification;
        }

        protected IActionResult CustomReturn<T>(T data = null) where T : class
        {
            var obj = new BaseResponseModel<T>
            {
                Success = !_notification.IsThereNotification(),
                Errors = _notification.GetNotifications(),
                Data = data,
                Message = _notification.GetMessage()
            };

            if (_notification.IsThereNotification())
                return BadRequest(obj);
            else
                return Ok(obj);
        }
    }
}
