namespace RapidPay.Api.Configuration
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILoggerFactory loggerFactory)
        {
            this.next = next;
            _logger = loggerFactory.CreateLogger<RequestLoggingMiddleware>();
        }

        public async Task Invoke(HttpContext context /* other dependencies */)
        {
            try
            {
                await next(context);
            }
            finally
            {
                _logger.LogInformation(
                    "Request {url} => {statusCode}",
                    context.Request?.Path.Value,
                    context.Response?.StatusCode);
            }
        }
    }
}
