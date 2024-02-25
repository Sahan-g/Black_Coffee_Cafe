using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace Black_Coffee_Cafe.Models
{
    public class CartVm
    {
        public List<CartItem>? Items { get; set; }

        public List<string>? Images { get; set; }

        public List<string>? ItemNames { get; set; }

        public double ? Total { get; set; }

        public int CartId { get; set; }

        [Required]
        public string PaymentMethod { get; set; }

    }
}
