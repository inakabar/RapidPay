namespace RapidPay.Api.Models
{
    public class BaseResponseModel<T>
    {
        public T Data { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
        public bool IsException { get; set; }
        public List<string> Errors { get; set; }
    }
}
