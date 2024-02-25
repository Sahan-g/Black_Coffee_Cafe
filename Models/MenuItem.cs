using Newtonsoft.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace Black_Coffee_Cafe.Models
{
    public class MenuItem
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public double Price { get; set; }
        [MaxLength(300,ErrorMessage ="Description should be less than 300 Charaters")]
        public string Description { get; set; }


        public string? CookingMethod0 { get; set; }

        public string? CookingMethod1 { get; set; }

        public string? Flavor1 { get; set; }
        public string? Flavor2 { get; set; }

        public string?  Topping0 { get; set; }
        public string? Topping1 { get; set; }
        public string? Topping2 { get; set; }
        public string? Topping3 { get; set; }

        public string?  ImageUrl{ get; set; }


    }
}
