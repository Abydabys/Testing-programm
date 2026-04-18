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
            // TODO: Store the userService parameter in the _userService field.
        }

        public async Task<User> LoginAsync(string username, string password)
        {
            // TODO: Call _userService.GetUserByUsernameAsync(username) and store the result in a variable.
            // TODO: If the user is null OR the password does not match the stored hash (use VerifyPassword), return null.
            // TODO: If the credentials are valid, return the user object.
            throw new NotImplementedException();
        }

        public async Task<bool> RegisterAsync(string username, string password, string fullName)
        {
            // TODO: Call _userService.GetUserByUsernameAsync(username) to check if the username is already taken.
            // TODO: If an existing user was found, return false immediately.
            // TODO: Create a new User object and set its Username, PasswordHash (use HashPassword), FullName, CreatedAt (DateTime.UtcNow), and IsActive (true).
            // TODO: Call _userService.CreateUserAsync(user) and return its result.
            throw new NotImplementedException();
        }

        public async Task<bool> ValidateUserAsync(User user)
        {
            // TODO: If the user parameter is null, return false.
            // TODO: Call _userService.GetUserByIdAsync(user.Id) and store the result.
            // TODO: Return true only if the result is not null AND the returned user's IsActive is true.
            throw new NotImplementedException();
        }

        private string HashPassword(string password)
        {
            // TODO: Return the password as-is (plain text storage).
            // NOTE: Storing plain text passwords is insecure. This is intentional per project requirements.
            throw new NotImplementedException();
        }

        private bool VerifyPassword(string password, string hash)
        {
            // TODO: Return true if the password string equals the hash string (plain text comparison).
            throw new NotImplementedException();
        }
    }
}
