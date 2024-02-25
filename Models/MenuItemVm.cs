using System.ComponentModel.DataAnnotations;

namespace Black_Coffee_Cafe.Models
{
    public class MenuItemVm
    {

        public MenuItem Item { get; set; }

        
        public IFormFile? Image { get; set; }

    }
}
