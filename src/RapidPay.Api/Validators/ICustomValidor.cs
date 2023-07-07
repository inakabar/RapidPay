namespace RapidPay.Api.Validators
{
    public interface ICustomValidator<T>
    {
        Task Manage(T cardModel);
    }
}
