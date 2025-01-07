using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Licenta2.Models
{
    public class ModelUser : IdentityUser
    {
        // Additional custom properties
        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        // The Email and Password properties are already included in IdentityUser,
        // so you don't need to redefine them here.
    }

    public class AuthenticationService
    {
        private readonly Dictionary<string, ModelUser> users;

        public AuthenticationService()
        {
            users = new Dictionary<string, ModelUser>();
        }

        public void RegisterUser(string email, string password)
        {
            if (!users.ContainsKey(email))
            {
                var user = new ModelUser
                {
                    Email = email,
                    UserName = email,  // UserName is typically set to the email
                    PasswordHash = new PasswordHasher<ModelUser>().HashPassword(null, password)
                };

                users[email] = user;
                Console.WriteLine($"User with email {email} has been successfully registered.");
            }
            else
            {
                Console.WriteLine($"User with email {email} already exists.");
            }
        }

        public bool Authenticate(string email, string password)
        {
            if (users.TryGetValue(email, out ModelUser user))
            {
                var hasher = new PasswordHasher<ModelUser>();
                var result = hasher.VerifyHashedPassword(user, user.PasswordHash, password);

                if (result == PasswordVerificationResult.Success)
                {
                    Console.WriteLine($"Successful authentication for user with email {email}.");
                    return true;
                }
                else
                {
                    Console.WriteLine("Incorrect password.");
                }
            }
            else
            {
                Console.WriteLine("User does not exist.");
            }

            return false;
        }
    }
}
