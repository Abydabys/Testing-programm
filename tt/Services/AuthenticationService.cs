using Microsoft.EntityFrameworkCore;
using tt.Data;
using tt.Models;

namespace tt.Services
{
    public class IAuthenticationService
    {
        private readonly TestingDbContext _context;

        public IAuthenticationService(TestingDbContext context)
        {
            _context = context;
        }

        public async Task<User> LoginAsync(string username, string password)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username
                                       && u.PasswordHash == password
                                       && u.IsActive);
        }

        public async Task<bool> RegisterAsync(string username, string password, string fullName)
        {
            bool exists = await _context.Users.AnyAsync(u => u.Username == username);
            if (exists) return false;

            _context.Users.Add(new User
            {
                Username     = username,
                PasswordHash = password,
                FullName     = fullName,
                IsActive     = true,
                CreatedAt    = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ValidateUserAsync(User user)
        {
            return await _context.Users.AnyAsync(u => u.Id == user.Id && u.IsActive);
        }
    }
}
