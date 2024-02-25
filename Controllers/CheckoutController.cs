using Black_Coffee_Cafe.Data;
using Black_Coffee_Cafe.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using Stripe.Checkout;
using System.Data.SqlClient;

namespace Black_Coffee_Cafe.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        public CheckoutController(IConfiguration configuration, ApplicationDbContext context = null, UserManager<User> userManager = null)
        {
            _context = context;
            _configuration = configuration;
            _userManager = userManager;
        }
        [Authorize(Roles ="User")]
        public async Task<IActionResult> Index(int id)
        {
            OrderItem orderItem = new OrderItem();
            orderItem.cartId = id;


            List<CartItem> cartItems = new List<CartItem>();

            await using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("defaultConnection")))
            {
                connection.Open();



                string selectQuery = @"SELECT * FROM CartItems WHERE Flag = 0 AND CartId = @CartId";

                using (SqlCommand command = new SqlCommand(selectQuery, connection))
                {
                    command.Parameters.AddWithValue("@CartId", orderItem.cartId);

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
                            cartItem.MenuItemcart = await _context.MenuItems.FindAsync(cartItem.MenuItemId);
                            cartItems.Add(cartItem);
                            orderItem.Total = orderItem.Total + cartItem.SubTotal;
                        }
                    }

                }
            }
            orderItem.CartItems = cartItems;



            return View(orderItem);
        }
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Checkout(int id)
        {

            OrderItem orderItem = new OrderItem();
            orderItem.cartId = id;


            List<CartItem> cartItems = new List<CartItem>();

            await using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("defaultConnection")))
            {
                connection.Open();



                string selectQuery = @"SELECT * FROM CartItems WHERE Flag = 0 AND CartId = @CartId";

                using (SqlCommand command = new SqlCommand(selectQuery, connection))
                {
                    command.Parameters.AddWithValue("@CartId", orderItem.cartId);

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
                            cartItem.MenuItemcart = await _context.MenuItems.FindAsync(cartItem.MenuItemId);
                            cartItems.Add(cartItem);
                            orderItem.Total = orderItem.Total + cartItem.SubTotal;
                        }
                    }

                }
            }
            orderItem.CartItems = cartItems;


            var domain = "https://localhost:44309/";



            var options = new SessionCreateOptions
            {
                SuccessUrl = domain + $"Checkout/OrderConfirmation/" + id,
                CancelUrl = domain + $"Home",
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                CustomerEmail = User.Identity.Name.ToString(),


            };


            foreach (var item in cartItems)
            {
                var sessionListItem = new SessionLineItemOptions()
                {
                    PriceData = new SessionLineItemPriceDataOptions()
                    {
                        UnitAmount = (long)(item.SubTotal * 100) / item.Quantity,
                        Currency = "lkr",
                        ProductData = new SessionLineItemPriceDataProductDataOptions()
                        {
                            Name = _context.MenuItems.Find(item.MenuItemId).Name.ToString(),

                        }
                    },
                    Quantity = item.Quantity,
                };
                options.LineItems.Add(sessionListItem);

            }

            var service = new SessionService();
            Session session = service.Create(options);
            TempData["Session"] = session.Id.ToString();

            Response.Headers.Add("Location", session.Url);

            return new StatusCodeResult(303);

        }
        [Authorize(Roles = "User")]
        public async Task<IActionResult> OrderConfirmation(int id)
        {
            if (TempData["Session"] != null)
            {
                var service = new SessionService();
                Session session = service.Get(TempData["Session"].ToString());

                OrderItem orderItem = new OrderItem();
                orderItem.cartId = id;

                List<CartItem> cartItems = new List<CartItem>();

                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("defaultConnection")))
                {
                    await connection.OpenAsync();

                    string selectQuery = @"SELECT * FROM CartItems WHERE Flag = 0 AND CartId = @CartId";

                    using (SqlCommand command = new SqlCommand(selectQuery, connection))
                    {
                        command.Parameters.AddWithValue("@CartId", orderItem.cartId);

                        using (SqlDataReader reader = await command.ExecuteReaderAsync())
                        {
                            while (await reader.ReadAsync())
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
                                cartItem.MenuItemcart = await _context.MenuItems.FindAsync(cartItem.MenuItemId);
                                cartItems.Add(cartItem);
                                orderItem.Total = orderItem.Total + cartItem.SubTotal;
                            }
                        }
                    }
                }

                orderItem.CartItems = cartItems;
                orderItem.OrderId = session.Id;
                orderItem.OrderStatus = "Pending"; // Processing, Ready to Collect/Serve
                orderItem.User_Id = _userManager.GetUserId(User);
                orderItem.OrderDate = DateTime.Today.ToString();
                orderItem.OrderTime = DateTime.Now.ToString("HH:mm:ss");
                orderItem.PaymentMethod = "Card";

                // Database - order table -- set cart items flag to 1
                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("defaultConnection")))
                {
                    await connection.OpenAsync();

                    // Create a SQL INSERT command
                    string insertCommand = "INSERT INTO Orders (OrderId, cartId, Total, UserId, PaymentMethod, OrderStatus, OrderDate, OrderTime) " +
                                           "VALUES (@OrderId, @cartId, @Total, @UserId, @PaymentMethod, @OrderStatus, @OrderDate, @OrderTime)";

                    using (SqlCommand command = new SqlCommand(insertCommand, connection))
                    {
                        // Set parameters for the INSERT command
                        command.Parameters.AddWithValue("OrderId", orderItem.OrderId);
                        command.Parameters.AddWithValue("@cartId", orderItem.cartId);
                        command.Parameters.AddWithValue("@Total", orderItem.Total);
                        command.Parameters.AddWithValue("@UserId", orderItem.User_Id);
                        command.Parameters.AddWithValue("@PaymentMethod", orderItem.PaymentMethod);
                        command.Parameters.AddWithValue("@OrderStatus", orderItem.OrderStatus);
                        command.Parameters.AddWithValue("@OrderDate", orderItem.OrderDate);
                        command.Parameters.AddWithValue("@OrderTime", orderItem.OrderTime);

                        await command.ExecuteNonQueryAsync();
                    }

                    string updateQuery = "UPDATE CartItems " +
                                         "SET Flag = 1, OrderId = @NewOrderId " +
                                         "WHERE Id = @ItemId";

                    foreach (var item in cartItems)
                    {
                        using (SqlCommand updateCommand = new SqlCommand(updateQuery, connection))
                        {
                            // Set parameters for the UPDATE command
                            updateCommand.Parameters.AddWithValue("@ItemId", item.Id);
                            updateCommand.Parameters.AddWithValue("@NewOrderId", orderItem.OrderId);

                            // Execute the UPDATE command
                            int rowsAffected = await updateCommand.ExecuteNonQueryAsync();

                            // Handle the result if needed
                        }
                    }

                    return View();
                }
            }

            return RedirectToAction("index", "Cart");
        }

    }
}