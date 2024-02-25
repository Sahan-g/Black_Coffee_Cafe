namespace Black_Coffee_Cafe.Models
{
    public class CartItem
    {
        public int Id { get; set; }
        public int  CartId{ get; set; }
        public MenuItem? MenuItemcart { get; set; }

        public int MenuItemId { get; set; }
        public double SubTotal { get; set; }
        public int Quantity { get; set; }
        public string? Notes { get; set; }
        public string? Flavor { get; set; }
        public string? Topping { get; set; }
        public string? CookingMethod  { get; set; }
        public string? OrderId { get; set; }
    }
}
