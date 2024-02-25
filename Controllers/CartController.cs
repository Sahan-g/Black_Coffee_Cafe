using Black_Coffee_Cafe.Data;
using Black_Coffee_Cafe.Migrations;
using Black_Coffee_Cafe.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration.UserSecrets;
using System.Data;
using System.Data.SqlClient;
using static Azure.Core.HttpHeader;

namespace Black_Coffee_Cafe.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _user;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        public CartController(ApplicationDbContext context, SignInManager<User> signInManager, UserManager<User> userManager, IConfiguration configuration)
        {
            _context = context;
            _signInManager = signInManager;
            _user = userManager;
            _configuration = configuration;
        }
       
        
        [Authorize(Roles ="User")]
        public async Task<IActionResult> Index()
        {
            var user = await _user.GetUserAsync(User);
            int cartId = -1; 

            await using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("defaultConnection")))
            {
                connection.Open();

                string query = "SELECT Id FROM Carts WHERE UserId = @UserId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", user.Id);

                    object result = await command.ExecuteScalarAsync();

                    if (result != null)
                    {
                        // If a CartId is found, convert it to the appropriate data type
                        cartId = Convert.ToInt32(result);
                    }   
                }
            }




            List<CartItem> cartItems=new List<CartItem>();
            List<string> Images = new List<string>();
            await using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("defaultConnection")))
            {
                connection.Open();

                 

                string selectQuery = @"SELECT * FROM CartItems WHERE Flag = 0 AND CartId = @CartId";

                using (SqlCommand command = new SqlCommand(selectQuery, connection))
                {
                    command.Parameters.AddWithValue("@CartId", cartId);

                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            CartItem cartItem = new CartItem
                            {
                                // Assuming the column order in the table matches the order in your SELECT query
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                CartId = reader.GetInt32(reader.GetOrdinal("CartId")),
                                MenuItemId = reader.GetInt32(reader.GetOrdinal("MenuItemId")),
                                SubTotal = reader.GetDouble(reader.GetOrdinal("SubTotal")),
                                Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                                Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                                Flavor = reader.GetString(reader.GetOrdinal("Flavor")),
                                Topping = reader.GetString(reader.GetOrdinal("Topping")),
                                CookingMethod = reader.GetString(reader.GetOrdinal("CookingMethod")),
                            };

                            cartItems.Add(cartItem);
                        }
                    }
                
                }
            }


            List<string> imageUrls = cartItems
            .Join(_context.MenuItems, cartItem => cartItem.MenuItemId, menuItem => menuItem.Id, (cartItem, menuItem) => menuItem.ImageUrl)
            .ToList();

            List<string> names= cartItems
            .Join(_context.MenuItems, cartItem => cartItem.MenuItemId, menuItem => menuItem.Id, (cartItem, menuItem) => menuItem.Name)
            .ToList();
            CartVm vm = new CartVm() {
                Items = cartItems,
                Images = imageUrls,
                ItemNames=names,
                CartId=cartId
            };
            //if (cartItems.Count != 0)
            //{
            //    vm.CartId = cartItems[0].Id;
            //}
            //else { vm.CartId = 0; }

                return View(vm);
        }
        [Authorize(Roles = "User")]
        public async Task<IActionResult> AddToCart(int Id)
        {
            var itemAddTocart = await _context.MenuItems.FindAsync(Id);
            CartItem item = new CartItem();
            item.MenuItemcart = itemAddTocart;
            var user = await _user.GetUserAsync(User);
            item.MenuItemId = itemAddTocart.Id;
            object result;
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("defaultConnection")))
            {
                connection.Open();

                string query = "SELECT Id FROM Carts WHERE UserId = @UserId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", user.Id);

                    result = command.ExecuteScalar();
                }
            }
            if (result != null && result != DBNull.Value)
            {
                item.CartId = (int)result;
            }
            else
            {
                await using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("defaultConnection")))
                {
                    await connection.OpenAsync();
                    string createCartSql = "INSERT INTO Carts (UserId) VALUES (@UserId)";
                    await using (SqlCommand command = new SqlCommand(createCartSql, connection))
                    {
                        command.Parameters.AddWithValue("@UserId", user.Id);
                        await command.ExecuteNonQueryAsync();
                    }
                }


            }
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("defaultConnection")))
            {
                connection.Open();

                string query = "SELECT Id FROM Carts WHERE UserId = @UserId";

                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserId", user.Id);

                    result = command.ExecuteScalar();
                }
                item.CartId = (int)result;
                return View(item);
            }
        }
        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> AddToCart(CartItem item)
        {


            await using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("defaultConnection")))
            {
                connection.Open();

                string insertQuery = @"INSERT INTO CartItems (CartId, MenuItemId, SubTotal, Quantity, Notes, Flavor, Topping, CookingMethod, Flag,OrderId)
                       VALUES (@CartId, @MenuItemId, @SubTotal, @Quantity, @Notes, @Flavor, @Topping, @CookingMethod, @Flag,@OrderId)";

               await using (SqlCommand command = new SqlCommand(insertQuery, connection))
                {
                    command.Parameters.AddWithValue("@CartId", item.CartId);
                    command.Parameters.AddWithValue("@MenuItemId", item.MenuItemId);
                    command.Parameters.AddWithValue("@SubTotal", item.SubTotal);
                    command.Parameters.AddWithValue("@Quantity", item.Quantity);
                    command.Parameters.Add("@Notes", SqlDbType.NVarChar).Value = (object)item.Notes ?? DBNull.Value;
                    command.Parameters.AddWithValue("@Flavor", item.Flavor);
                    command.Parameters.AddWithValue("@Topping", item.Topping);
                    command.Parameters.AddWithValue("@CookingMethod", item.CookingMethod);
                    command.Parameters.AddWithValue("@Flag", 0);
                    command.Parameters.AddWithValue("@OrderId", "None");
                    command.ExecuteNonQuery();

                } 
            }
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> EditCartItem(int itemId)
        {
            CartItem item = null;

            try
            {
                
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("defaultConnection")))
                {
                    await connection.OpenAsync();

                    string query = "SELECT * FROM CartItems WHERE Id = @Id";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@Id", itemId);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            if (await reader.ReadAsync())
                            {
                                item = new CartItem
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                    CartId = reader.GetInt32(reader.GetOrdinal("CartId")),
                                    MenuItemId = reader.GetInt32(reader.GetOrdinal("MenuItemId")),
                                    SubTotal = reader.GetDouble(reader.GetOrdinal("SubTotal")),
                                    Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                                    Notes = reader.IsDBNull(reader.GetOrdinal("Notes")) ? null : reader.GetString(reader.GetOrdinal("Notes")),
                                    Flavor = reader.GetString(reader.GetOrdinal("Flavor")),
                                    Topping = reader.GetString(reader.GetOrdinal("Topping")),
                                    CookingMethod = reader.GetString(reader.GetOrdinal("CookingMethod")),
                                    OrderId=reader.GetString(reader.GetOrdinal("OrderId")),
                                };
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle the exception, log it, or return an error response
                Console.WriteLine($"Error: {ex.Message}");
                // You might want to return a specific view or an error response
                return View("Error");
            }

            if (item == null)
            {
                // Handle the case where the item with the specified ItemId is not found
                return NotFound();
            }

            item.MenuItemcart = await _context.MenuItems.FindAsync(item.MenuItemId);

            return View(item);
        }
        [Authorize(Roles = "User")]
        [HttpPost]
        public async Task<IActionResult> EditCartItem(CartItem updatedItem)
        {
            int Flag = 0;
            int Id = updatedItem.Id;
            try
            {
                await using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("defaultConnection")))
                {
                    connection.Open();

                    string updateQuery = @"UPDATE CartItems
                                   SET SubTotal = @SubTotal,
                                       Quantity = @Quantity,
                                       Notes = @Notes,
                                       Flavor = @Flavor,
                                       Topping = @Topping,
                                       CookingMethod = @CookingMethod,
                                       Flag = @Flag,
                                       OrderId=@OrderId
                                   WHERE Id = @Id";

                    using (SqlCommand command = new SqlCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@SubTotal", updatedItem.SubTotal);
                        command.Parameters.AddWithValue("@Quantity", updatedItem.Quantity);
                        command.Parameters.Add("@Notes", SqlDbType.NVarChar).Value = (object)updatedItem.Notes ?? DBNull.Value;
                        command.Parameters.AddWithValue("@Flavor", updatedItem.Flavor);
                        command.Parameters.AddWithValue("@Topping", updatedItem.Topping);
                        command.Parameters.AddWithValue("@CookingMethod", updatedItem.CookingMethod);
                        command.Parameters.AddWithValue("@Id", Id);
                        command.Parameters.AddWithValue("@Flag", Flag);
                        command.Parameters.AddWithValue("@OrderID", updatedItem.OrderId);

                        command.ExecuteNonQuery();
                    }
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Handle the exception, log it, or return an error response
                Console.WriteLine($"Error: {ex.Message}");
                // You might want to return a specific view or an error response
                return View("Error");
            }
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> DeleteCartItem(int itemId)
        {
            try
            {
                await using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("defaultConnection")))
                {
                    connection.Open();

                    string updateQuery = @"UPDATE CartItems
                                   SET Flag = 2
                                   WHERE Id = @Id";

                    using (SqlCommand command = new SqlCommand(updateQuery, connection))
                    {
                        command.Parameters.AddWithValue("@Id", itemId);
                        command.ExecuteNonQuery();
                    }
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                
                Console.WriteLine($"Error: {ex.Message}");
           
                return View("Error");
            }
        }


        


    }
}
