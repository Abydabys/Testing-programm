using tt.Models;
using System.Security.Cryptography;
using System.Text;

namespace tt.Services
{
    public interface IAuthenticationService
    {
        Task<User> LoginAsync(string username, string password);
        Task<bool> RegisterAsync(string username, string password, string fullName);
        Task<bool> ValidateUserAsync(User user);
    }

    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserService _userService;

        public AuthenticationService(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<User> LoginAsync(string username, string password)
        {
            var user = await _userService.GetUserByUsernameAsync(username);
            if (user == null || !VerifyPassword(password, user.PasswordHash))
                return null;

            return user;
        }

        public async Task<bool> RegisterAsync(string username, string password, string fullName)
        {
            var existingUser = await _userService.GetUserByUsernameAsync(username);
            if (existingUser != null)
                return false;

            var user = new User
            {
                Username = username,
                PasswordHash = HashPassword(password),
                FullName = fullName,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            return await _userService.CreateUserAsync(user);
        }

        public async Task<bool> ValidateUserAsync(User user)
        {
            if (user == null)
                return false;

            var dbUser = await _userService.GetUserByIdAsync(user.Id);
            return dbUser != null && dbUser.IsActive;
        }

        // Вспомогательные методы для хеширования пароля
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(bytes);
            }
        }

        private bool VerifyPassword(string password, string hash)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedInput = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var hashedInputStr = Convert.ToBase64String(hashedInput);
                return hashedInputStr == hash;
            }
        }
    }
}
