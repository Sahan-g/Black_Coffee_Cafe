namespace Black_Coffee_Cafe.Models
{
    public class OrderItem
    {
        public string OrderId { get; set; }
        public List<CartItem>? CartItems { get; set; }
        public int cartId { get; set; }

        public double Total { get; set; }
        public string? User_Id { get; set; }
        public User? User { get; set; }
        public string?  PaymentMethod { get; set; }

        public string? OrderStatus { get; set; }

        public string? OrderDate { get; set; }
        public string? OrderTime { get; set; }
     }
}
