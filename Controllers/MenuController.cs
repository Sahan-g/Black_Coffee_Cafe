using Black_Coffee_Cafe.Data;
using Black_Coffee_Cafe.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.CompilerServices;

namespace Black_Coffee_Cafe.Controllers
{
    public class MenuController : Controller
    {
        private readonly ApplicationDbContext _Context;
        private readonly IWebHostEnvironment _HostEnvironment;
        public MenuController(ApplicationDbContext dbContext, IWebHostEnvironment hostEnvironment = null)
        {
            _Context = dbContext;
            _HostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {

            List<MenuItem> items = new List<MenuItem>();
            items = _Context.MenuItems.ToList();
            return View(items);
        }
        [Authorize(Roles = "Admin")]
        public IActionResult AddItem() {

            MenuItemVm vm = new MenuItemVm();

            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> AddItem(MenuItemVm vm)
        {

            
            if (vm.Image != null)
            {
                vm.Item.ImageUrl = uploadFile(vm.Image);


            }
            await _Context.MenuItems.AddAsync(vm.Item);
            await _Context.SaveChangesAsync();


            return RedirectToAction("Index");

        }

        private string uploadFile(IFormFile image)
        {
            string filename = " ";
            if (image != null)
            {
                string uploadDirLocation = Path.Combine(_HostEnvironment.WebRootPath, "Images");
                filename = Guid.NewGuid().ToString() + "_" + image.FileName;
                string filePath = Path.Combine(uploadDirLocation, filename);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    image.CopyTo(fileStream);
                }
            }
            return filename;

        }

        [Authorize(Roles = "Admin")]
        public IActionResult EditItem(int id) {

            MenuItemVm vm = new MenuItemVm();

            vm.Item = _Context.MenuItems.First(i => i.Id == id);
                
        
        
            if (vm.Item.Id != null)
            {

                return View(vm);
            }
                return RedirectToAction("Index");
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult EditItem(MenuItemVm vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var itemToEdit = _Context.MenuItems.FirstOrDefault(u => u.Id == vm.Item.Id);

            if (itemToEdit == null)
            {
                return View();
            }

            itemToEdit.Name = vm.Item.Name;
            itemToEdit.Price = vm.Item.Price;
            itemToEdit.Description = vm.Item.Description;
            itemToEdit.CookingMethod0 = vm.Item.CookingMethod0;
            itemToEdit.CookingMethod1 = vm.Item.CookingMethod1;
            itemToEdit.Flavor1 = vm.Item.Flavor1;
            itemToEdit.Flavor2 = vm.Item.Flavor2;
            itemToEdit.Topping0 = vm.Item.Topping0;
            itemToEdit.Topping1 = vm.Item.Topping1;
            itemToEdit.Topping2 = vm.Item.Topping2;
            itemToEdit.Topping3 = vm.Item.Topping3;

            if (vm.Image != null)
            {
                itemToEdit.ImageUrl = uploadFile(vm.Image);
            }
            _Context.Update(itemToEdit);
            _Context.SaveChanges();

            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        public IActionResult DeleteItem(int id)
        {
            MenuItemVm vm = new MenuItemVm();

            vm.Item = _Context.MenuItems.First(i => i.Id == id);



            if (vm.Item.Id != null)
            {

                return View(vm);
            }
            return RedirectToAction("Index");

        }


        [HttpPost]
        public IActionResult  DeleteItemPost(int id)
        {
            var itemToDelete = _Context.MenuItems.FirstOrDefault(u => u.Id == id);

            if (itemToDelete == null)
            {
                return NotFound(); // Or handle the case where the item with the given id is not found
            }

            _Context.MenuItems.Remove(itemToDelete);
            _Context.SaveChanges();

            return RedirectToAction("Index");
        }


    }




}
