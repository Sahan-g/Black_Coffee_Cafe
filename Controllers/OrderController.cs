using Black_Coffee_Cafe.Data;
using Black_Coffee_Cafe.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data.SqlClient;
using System.Security.Claims;

namespace Black_Coffee_Cafe.Controllers
{
    public class OrderController : Controller
    {

        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;
        public OrderController(IConfiguration configuration, UserManager<User> userManager = null, ApplicationDbContext context = null)
        {
            _configuration = configuration;
            _userManager = userManager;
            _context = context;
        }
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Index()
        {



            //    using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("defaultConnection")))
            //    {
            //        await connection.OpenAsync();

            //        string selectQuery = @"SELECT * FROM Orders WHERE UserId = @UserId";

            //        using (SqlCommand command = new SqlCommand(selectQuery, connection))
            //        {
            //            command.Parameters.AddWithValue("@UserId", User.Identity.ToString());

            //            List<OrderItem> userOrders = new List<OrderItem>();

            //            using (SqlDataReader reader = await command.ExecuteReaderAsync())
            //            {
            //                while (await reader.ReadAsync())
            //                {
            //                    OrderItem orderItem = new OrderItem
            //                    {
            //                        OrderId = reader.GetString(reader.GetOrdinal("OrderId")),
            //                        cartId = reader.GetInt32(reader.GetOrdinal("cartId")),
            //                        Total = reader.GetDouble(reader.GetOrdinal("Total")),
            //                        User_Id = reader.GetString(reader.GetOrdinal("UserId")),
            //                        PaymentMethod = reader.GetString(reader.GetOrdinal("PaymentMethod")),
            //                        OrderStatus = reader.GetString(reader.GetOrdinal("OrderStatus")),
            //                        OrderDate = reader.GetString(reader.GetOrdinal("OrderDate")),
            //                        OrderTime = reader.GetString(reader.GetOrdinal("OrderTime")),
            //                        // Add other properties as needed
            //                    };

            //                    userOrders.Add(orderItem);
            //                }
            //            }

            //            // Do something with the userOrders list, e.g., pass it to a view
            //            return View(userOrders);
            //        }


            //    }
            List<OrderItem> userOrders = new List<OrderItem>();

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("defaultConnection")))
            {
                await connection.OpenAsync();

                string selectQuery = @"
        SELECT OrderId, cartId, Total, UserId, PaymentMethod, OrderStatus, OrderDate, OrderTime
        FROM Orders
        WHERE UserId = @UserId";

                using (SqlCommand command = new SqlCommand(selectQuery, connection))
                {
                    command.Parameters.AddWithValue("@UserId", _userManager.GetUserId(User));

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            OrderItem orderItem = new OrderItem
                            {
                                OrderId = reader.GetString(reader.GetOrdinal("OrderId")),
                                cartId = reader.GetInt32(reader.GetOrdinal("cartId")),
                                Total = reader.GetDouble(reader.GetOrdinal("Total")),
                                User_Id = reader.GetString(reader.GetOrdinal("UserId")),
                                PaymentMethod = reader.GetString(reader.GetOrdinal("PaymentMethod")),
                                OrderStatus = reader.GetString(reader.GetOrdinal("OrderStatus")),
                                OrderDate = reader.GetString(reader.GetOrdinal("OrderDate")),
                                OrderTime = reader.GetString(reader.GetOrdinal("OrderTime")),
                            };

                            userOrders.Add(orderItem);
                        }
                    }
                }

                foreach (var order in userOrders)
                {
                    string selectCartItemsQuery = @"
            SELECT Id, CartId, MenuItemId, SubTotal, Quantity, Notes, Flavor, Topping, CookingMethod, OrderId
            FROM CartItems
            WHERE OrderId = @OrderId";

                    List<CartItem> cartItems = new List<CartItem>();

                    using (SqlCommand cartItemsCommand = new SqlCommand(selectCartItemsQuery, connection))
                    {
                        cartItemsCommand.Parameters.AddWithValue("@OrderId", order.OrderId);

                        using (SqlDataReader cartItemsReader = await cartItemsCommand.ExecuteReaderAsync())
                        {
                            while (await cartItemsReader.ReadAsync())
                            {
                                CartItem cartItem = new CartItem
                                {
                                    Id = cartItemsReader.GetInt32(cartItemsReader.GetOrdinal("Id")),
                                    CartId = cartItemsReader.GetInt32(cartItemsReader.GetOrdinal("CartId")),
                                    MenuItemId = cartItemsReader.GetInt32(cartItemsReader.GetOrdinal("MenuItemId")),
                                    SubTotal = cartItemsReader.GetDouble(cartItemsReader.GetOrdinal("SubTotal")),
                                    Quantity = cartItemsReader.GetInt32(cartItemsReader.GetOrdinal("Quantity")),
                                    Notes = cartItemsReader.IsDBNull(cartItemsReader.GetOrdinal("Notes")) ? null : cartItemsReader.GetString(cartItemsReader.GetOrdinal("Notes")),
                                    Flavor = cartItemsReader.GetString(cartItemsReader.GetOrdinal("Flavor")),
                                    Topping = cartItemsReader.GetString(cartItemsReader.GetOrdinal("Topping")),
                                    CookingMethod = cartItemsReader.GetString(cartItemsReader.GetOrdinal("CookingMethod")),
                                    OrderId = cartItemsReader.GetString(cartItemsReader.GetOrdinal("OrderId")),
                                };

                                // Fetch associated menu item for the current cart item
                                cartItem.MenuItemcart = await _context.MenuItems.FindAsync(cartItem.MenuItemId);

                                cartItems.Add(cartItem);
                            }
                        }
                    }

                    // Set the cart items for the current order
                    order.CartItems = cartItems;
                }
            }

            return View(userOrders);



        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminOrder()
        {
            List<OrderItem> userOrders = new List<OrderItem>();

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("defaultConnection")))
            {
                await connection.OpenAsync();

                string selectQuery = @"SELECT * FROM Orders";

                using (SqlCommand command = new SqlCommand(selectQuery, connection))
                {
                    command.Parameters.AddWithValue("@UserId", _userManager.GetUserId(User));

                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            OrderItem orderItem = new OrderItem
                            {
                                OrderId = reader.GetString(reader.GetOrdinal("OrderId")),
                                cartId = reader.GetInt32(reader.GetOrdinal("cartId")),
                                Total = reader.GetDouble(reader.GetOrdinal("Total")),
                                User_Id = reader.GetString(reader.GetOrdinal("UserId")),
                                PaymentMethod = reader.GetString(reader.GetOrdinal("PaymentMethod")),
                                OrderStatus = reader.GetString(reader.GetOrdinal("OrderStatus")),
                                OrderDate = reader.GetString(reader.GetOrdinal("OrderDate")),
                                OrderTime = reader.GetString(reader.GetOrdinal("OrderTime")),
                            };

                            userOrders.Add(orderItem);
                        }
                    }
                }

                foreach (var order in userOrders)
                {
                    string selectCartItemsQuery = @"
            SELECT Id, CartId, MenuItemId, SubTotal, Quantity, Notes, Flavor, Topping, CookingMethod, OrderId
            FROM CartItems
            WHERE OrderId = @OrderId";

                    List<CartItem> cartItems = new List<CartItem>();

                    using (SqlCommand cartItemsCommand = new SqlCommand(selectCartItemsQuery, connection))
                    {
                        cartItemsCommand.Parameters.AddWithValue("@OrderId", order.OrderId);

                        using (SqlDataReader cartItemsReader = await cartItemsCommand.ExecuteReaderAsync())
                        {
                            while (await cartItemsReader.ReadAsync())
                            {
                                CartItem cartItem = new CartItem
                                {
                                    Id = cartItemsReader.GetInt32(cartItemsReader.GetOrdinal("Id")),
                                    CartId = cartItemsReader.GetInt32(cartItemsReader.GetOrdinal("CartId")),
                                    MenuItemId = cartItemsReader.GetInt32(cartItemsReader.GetOrdinal("MenuItemId")),
                                    SubTotal = cartItemsReader.GetDouble(cartItemsReader.GetOrdinal("SubTotal")),
                                    Quantity = cartItemsReader.GetInt32(cartItemsReader.GetOrdinal("Quantity")),
                                    Notes = cartItemsReader.IsDBNull(cartItemsReader.GetOrdinal("Notes")) ? null : cartItemsReader.GetString(cartItemsReader.GetOrdinal("Notes")),
                                    Flavor = cartItemsReader.GetString(cartItemsReader.GetOrdinal("Flavor")),
                                    Topping = cartItemsReader.GetString(cartItemsReader.GetOrdinal("Topping")),
                                    CookingMethod = cartItemsReader.GetString(cartItemsReader.GetOrdinal("CookingMethod")),
                                    OrderId = cartItemsReader.GetString(cartItemsReader.GetOrdinal("OrderId")),
                                };

                                // Fetch associated menu item for the current cart item
                                cartItem.MenuItemcart = await _context.MenuItems.FindAsync(cartItem.MenuItemId);

                                cartItems.Add(cartItem);
                            }
                        }
                    }

                    // Set the cart items for the current order
                    order.CartItems = cartItems;
                }
            }
            userOrders = userOrders.OrderByDescending(o => o.OrderStatus != "Pending").ThenByDescending(o => o.OrderDate).ToList();


            var viewModel = new AdminOrderVM
            {
                Orders = userOrders,
                StatusOptions = GetOrderStatusOptions(),
                StatusUpdateModel = new OrderStatusUpdateModel(),
            };


            return View(viewModel);


        }

        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(OrderStatusUpdateModel statusUpdateModel)
        {
            if (ModelState.IsValid)
            {
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("defaultConnection")))
                {
                    await connection.OpenAsync();

                    // Create an update query
                    string updateQuery = "UPDATE Orders SET OrderStatus = @NewStatus WHERE OrderId = @OrderId";

                    using (SqlCommand command = new SqlCommand(updateQuery, connection))
                    {
                        // Add parameters
                        command.Parameters.AddWithValue("@NewStatus", statusUpdateModel.NewStatus);
                        command.Parameters.AddWithValue("@OrderId", statusUpdateModel.OrderId);

                        // Execute the update query
                        await command.ExecuteNonQueryAsync();
                    }
                }

                return RedirectToAction("AdminOrder");
            }

            // If ModelState is not valid, return to the view with errors
            TempData["ErrorMessage"] = "Error updating order status.";
            return RedirectToAction("AdminOrder");
        }


        private List<SelectListItem> GetOrderStatusOptions()
        {
            // Customize this method based on your available order statuses
            return new List<SelectListItem>
    {
        new SelectListItem { Value = "Pending", Text = "Pending" },
        new SelectListItem { Value = "Processing", Text = "Processing" },
        new SelectListItem { Value = "Ready", Text = "Ready" },
        new SelectListItem{Value="Finished",Text="Finished"}
        // Add more items as needed
    };
        }
    }
}