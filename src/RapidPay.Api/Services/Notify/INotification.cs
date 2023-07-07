namespace RapidPay.Api.Services.Notify
{
    public interface INotification
    {
        void AddNotification(string message);
        void AddMessage(string message);

        bool IsThereNotification();

        List<string> GetNotifications();
        string GetMessage();
    }
}
