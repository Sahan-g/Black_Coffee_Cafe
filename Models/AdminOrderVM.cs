using Microsoft.AspNetCore.Mvc.Rendering;

namespace Black_Coffee_Cafe.Models
{
    public class AdminOrderVM
    {
        public List<OrderItem> Orders { get; set; }
        public List<SelectListItem> StatusOptions { get; set; }
        public OrderStatusUpdateModel StatusUpdateModel { get; set; }
    }
}
