using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata.Ecma335;

namespace Black_Coffee_Cafe.Models
{
    public class UserCart
    {
        [Key]
        public int Id{ get; set; }

        public int? ItemId { get; set; }
        [ForeignKey(nameof(ItemId))]

        public MenuItem Item { get; set; }

        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]

        public User User { get; set; }

        public int Quantity { get; set; }
        [NotMapped]

        public double Price { get; set; }


    }
}
