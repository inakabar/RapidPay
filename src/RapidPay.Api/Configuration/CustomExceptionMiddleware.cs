using RapidPay.Api.Models;
using System.Diagnostics;
using System.Net;
using System.Text.Json;

namespace RapidPay.Api.Configuration
{
    public class CustomExceptionMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<CustomExceptionMiddleware> _logger;

        public CustomExceptionMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            this.next = next;
            _logger = loggerFactory.CreateLogger<CustomExceptionMiddleware>();
        }

        public async Task Invoke(HttpContext context /* other dependencies */)
        {
            try
            {
                await next(context);
            }
            catch (Exception exceptionObj)
            {
                _logger.LogCritical(exceptionObj, null, null);
                await HandleExceptionAsync(context, exceptionObj);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var result = new BaseResponseModel<object>()
            {
                Errors = new List<string>() { exception.Message },
                Success = false,
                IsException = true,
                Message = exception.Message,
                Data = new
                {
                    StackTrace = exception.StackTrace,
                    Message = exception.InnerException?.Message ?? exception.Message
                }
            };

            await context.Response.WriteAsJsonAsync(result);
        }
    }
}
