using System.ComponentModel.DataAnnotations;

namespace RapidPay.Api.Models
{
    public class MakePaymentModel
    {
        [Required]
        [MaxLength(15)]
        [MinLength(15)]
        public string CardNumber { get; set; }
        public double PaymentValue { get; set; }
    }
}
