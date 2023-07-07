using System;
using System.ComponentModel.DataAnnotations;

namespace RapidPay.DataAccess.Entities
{
    public class Card
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        [MaxLength(15)]
        [MinLength(15)]
        public string Number { get; set; }

        [Required]
        public Double Balance { get; set; }

        [Required]
        public DateTime CreateDate { get; set; }

        [Required]
        public DateTime LastUpdateDate { get; set; }

        [Required]
        public bool Active { get; set; }
    }
}
