using System;
using System.ComponentModel.DataAnnotations;

namespace RapidPay.DataAccess.Entities
{
    public class PaymentHistory
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public Guid PaymentId { get; set; }

        [Required]
        [MaxLength(15)]
        [MinLength(15)]
        public string CardNumber { get; set; }

        [Required]
        public Double Value { get; set; }

        // May be null cause the CardNumber was not found
        public Double? CurrentlyBalance { get; set; }

        [Required]
        public Double Fee { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public bool Success { get; set; }
        [Required]
        public string Message { get; set; }
    }
}
