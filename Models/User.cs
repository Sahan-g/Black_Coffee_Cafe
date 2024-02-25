using Microsoft.AspNetCore.Identity;

namespace Black_Coffee_Cafe.Models
{
    public class User:IdentityUser
    {
        public User()
        {
        }

        public User(string firstName, string lastName )
        {
            FirstName = firstName;
            LastName = lastName;
            
        }

        public string FirstName { get; set; }

        public  string LastName { get; set; }

        public string Email { get; set; }




    }
}
