using Microsoft.EntityFrameworkCore;
using tt.Data;
using tt.Models;

namespace tt.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly TestingDbContext _context;

        public AuthenticationService(TestingDbContext context)
        {
            _context = context;
        }

        // Войти: ищем пользователя по логину и паролю (без шифрования)
        public async Task<User> LoginAsync(string username, string password)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username
                                       && u.PasswordHash == password
                                       && u.IsActive);
        }

        // Зарегистрировать нового пользователя
        // Возвращает false если логин уже занят
        public async Task<bool> RegisterAsync(string username, string password, string fullName)
        {
            bool exists = await _context.Users.AnyAsync(u => u.Username == username);
            if (exists) return false;

            _context.Users.Add(new User
            {
                Username = username,
                PasswordHash = password, // храним как есть, без хеширования
                FullName = fullName,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            });

            await _context.SaveChangesAsync();
            return true;
        }

        // Проверить что пользователь с таким ID существует в базе
        public async Task<bool> ValidateUserAsync(User user)
        {
            return await _context.Users.AnyAsync(u => u.Id == user.Id && u.IsActive);
        }
    }
}