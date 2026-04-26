using Microsoft.EntityFrameworkCore;
using tt.Data;
using tt.Models;

namespace tt.Services
{
    // Сервис управления пользователями — напрямую через базу данных
    public class UserService
    {
        private readonly TestingDbContext _context;

        public UserService(TestingDbContext context)
        {
            _context = context;
        }

        // Получить всех активных пользователей
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users
                .Where(u => u.IsActive)
                .ToListAsync();
        }

        // Найти пользователя по ID
        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        // Найти пользователя по логину
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        // Создать нового пользователя
        public async Task<User> CreateUserAsync(string username, string password, string fullName)
        {
            var user = new User
            {
                Username = username,
                PasswordHash = password, // без хеширования
                FullName = fullName,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        // Обновить данные пользователя
        public async Task<bool> UpdateUserAsync(User user)
        {
            var existing = await _context.Users.FindAsync(user.Id);
            if (existing == null) return false;

            existing.Username = user.Username;
            existing.FullName = user.FullName;

            await _context.SaveChangesAsync();
            return true;
        }

        // Деактивировать пользователя (мягкое удаление)
        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return false;

            user.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        // Проверить, занят ли логин
        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }
    }
}