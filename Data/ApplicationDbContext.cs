using Black_Coffee_Cafe.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Black_Coffee_Cafe.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
             
        }

        public DbSet <User>  Users{ get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }

        public DbSet<UserCart> UserCarts { get; set; }
    }
}
