using Microsoft.EntityFrameworkCore;
using tt.Data;
using tt.Models;

namespace tt.Services
{
    // —ервис дл€ работы с пользовател€ми (получение, создание, обновление)
    public class UserService
    {
        private readonly TestingDbContext _context;

        // ѕолучаем контекст базы данных через конструктор
        public UserService(TestingDbContext context)
        {
            _context = context;
        }

        // ѕолучить всех пользователей из базы данных
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        // Ќайти пользовател€ по его ID
        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        // Ќайти пользовател€ по имени пользовател€ (логину)
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        // —оздать нового пользовател€ и сохранить в базе данных
        public async Task<User> CreateUserAsync(string username, string password, string role = "Student")
        {
            var user = new User
            {
                Username = username,
                // ’ешируем пароль перед сохранением (не храним в открытом виде)
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = role,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        // ќбновить данные существующего пользовател€
        public async Task<bool> UpdateUserAsync(User user)
        {
            var existing = await _context.Users.FindAsync(user.Id);
            if (existing == null)
                return false;

            // ќбновл€ем только те пол€, которые можно мен€ть
            existing.Username = user.Username;
            existing.Role = user.Role;

            await _context.SaveChangesAsync();
            return true;
        }

        // ”далить пользовател€ по ID
        public async Task<bool> DeleteUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        // ѕроверить, существует ли пользователь с таким логином
        public async Task<bool> UsernameExistsAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username);
        }
    }
}
