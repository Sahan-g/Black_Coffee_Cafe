using Black_Coffee_Cafe.Data;
using Black_Coffee_Cafe.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Data.SqlClient;

namespace Black_Coffee_Cafe.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;

        public ReviewsController(IConfiguration configuration, UserManager<User> userManager = null, ApplicationDbContext context = null)
        {
            _configuration = configuration;
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            List<Review> reviews = new List<Review>();

            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("defaultConnection")))
            {
                await connection.OpenAsync();

                string selectQuery = "SELECT ReviewId, UserId, Date, Time, ReviewDescription FROM Reviews";

                using (SqlCommand command = new SqlCommand(selectQuery, connection))
                {
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            Review review = new Review
                            {
                                ReviewId = reader.GetInt32(reader.GetOrdinal("ReviewId")),
                                UserId = reader.GetString(reader.GetOrdinal("UserId")),
                                DateString = reader.GetString(reader.GetOrdinal("Date")),
                                Time = reader.GetString(reader.GetOrdinal("Time")),
                                ReviewDescription = reader.GetString(reader.GetOrdinal("ReviewDescription")),
                            };


                            User user =await _context.Users.FindAsync(review.UserId) ;
                            review.User = user;
                            reviews.Add(review);
                        }
                    }
                }
            }

            return View(reviews);
        }

        public IActionResult AddReview()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddReview(Review review)
        {

            review.UserId = _userManager.GetUserId(User);
            review.DateString = DateTime.Today.Date.ToString();
            review.Time= DateTime.Now.TimeOfDay.ToString();
            

                using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("defaultConnection")))
                {
                    await connection.OpenAsync();

                    string insertQuery = @"
                            INSERT INTO Reviews (UserId, Date, Time, ReviewDescription)
                            VALUES (@userid, @date, @Time, @ReviewDescription);
                        ";

                    using (SqlCommand command = new SqlCommand(insertQuery, connection))
                    {
                        command.Parameters.AddWithValue("@userid", review.UserId);
                        command.Parameters.AddWithValue("@Date", review.DateString);
                        command.Parameters.AddWithValue("@Time", review.Time);
                        command.Parameters.AddWithValue("@ReviewDescription", review.ReviewDescription);

                        await command.ExecuteNonQueryAsync();
                    }
                }
            return RedirectToAction("Index");
        }
         
        
    }
}
