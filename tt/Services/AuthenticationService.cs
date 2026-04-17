using tt.Models;

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

        // Helper methods for password handling (plain text comparison)
        private string HashPassword(string password)
        {
            // WARNING: Storing plain text passwords is insecure.
            // This implementation stores passwords in plain text per user request.
            return password;
        }

        private bool VerifyPassword(string password, string hash)
        {
            return password == hash;
        }
    }
}
