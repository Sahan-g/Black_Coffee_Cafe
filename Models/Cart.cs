namespace Black_Coffee_Cafe.Models
{
    public class Cart
    {
        public int CartId { get; set; }

        public List<CartItem> Items { get; set;}
    }
}
