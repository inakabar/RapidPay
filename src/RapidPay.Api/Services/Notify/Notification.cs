namespace RapidPay.Api.Services.Notify
{
    public class Notification : INotification
    {
        private readonly ILogger _logger;
        private List<string> _notifications;
        private string _message;

        public Notification(ILogger<Notification> logger)
        {
            _logger = logger;
        }

        public void AddNotification(string message)
        {
            if(_notifications == null) this._notifications = new List<string>();

            if (!string.IsNullOrEmpty(message))
            {
                _notifications.Add(message);
                _logger.LogError(message);
            }
        }

        public void AddMessage(string message)
        {
            _logger.LogInformation(message);
            _message = message;
        }

        public List<string> GetNotifications()
        {
            return IsThereNotification() ? _notifications : new List<string>();
        }
        
        public string GetMessage()
        {
            return IsThereNotification() ? "Operation Not Successful" : _message;
        }

        public bool IsThereNotification()
        {
            return _notifications != null && _notifications.Any();
        }
    }
}
