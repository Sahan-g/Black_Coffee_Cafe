using Black_Coffee_Cafe.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Black_Coffee_Cafe.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        public HomeController(ILogger<HomeController> logger, SignInManager<User> signInManager, IConfiguration configuration, UserManager<User> userManager)
        {
            _logger = logger;
            _signInManager = signInManager;
            _configuration = configuration;
            _userManager = userManager;
        }

        public async  Task<IActionResult>  Index()
        {
            if (_signInManager.IsSignedIn(User))
            {
                var user = await _userManager.GetUserAsync(User);

                if (user != null && !await _userManager.IsInRoleAsync(user, "Admin") && !await _userManager.IsInRoleAsync(user, "User"))
                {
                     

                    if (user != null )
                    {
                        await _userManager.AddToRoleAsync(user, "User");
                    }
                    object result;
                    await using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("defaultConnection")))
                    {
                        connection.Open();

                        string query = "SELECT Id FROM Carts WHERE UserId = @UserId";

                        
                        await using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@UserId", user.Id);

                             result = command.ExecuteScalar();
                            
                        }

                       
                    }
                    if (result != null && result != DBNull.Value)
                    {
                        int cartId = (int)result;

                        // The user has a CartId (cartId variable contains the CartId)
                        // Add your logic here
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
                        // The user does not have a CartId
                        // Add your logic here
                    }

                }
            
                
            
            
            }




            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public async Task CreateCartForUser(string userId)
        {
            await using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("defaultConnection")))
            {
                await connection.OpenAsync();
                string createCartSql = "INSERT INTO UserCarts (UserId) VALUES (@UserId)";
               await using (SqlCommand command = new SqlCommand(createCartSql, connection))
                {
                    command.Parameters.AddWithValue("@UserId", userId);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }

    }
}
