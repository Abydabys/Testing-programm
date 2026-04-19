using Microsoft.EntityFrameworkCore;
using tt.Data;
using tt.Models;

namespace tt.Services
{
    // Сервис для входа в систему и проверки данных пользователя
    public class AuthenticationService
    {
        private readonly TestingDbContext _context;

        // Храним текущего вошедшего пользователя (пока приложение запущено)
        public User? CurrentUser { get; private set; }

        public AuthenticationService(TestingDbContext context)
        {
            _context = context;
        }

        // Войти в систему: проверяем логин и пароль
        // Возвращает true если данные верны, false если нет
        public async Task<bool> LoginAsync(string username, string password)
        {
            // Ищем пользователя по логину
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username);

            if (user == null)
                return false; // Такого пользователя нет

            // Проверяем пароль (сравниваем с хешем в базе данных)
            bool passwordCorrect = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);

            if (passwordCorrect)
            {
                // Запоминаем текущего пользователя
                CurrentUser = user;
                return true;
            }

            return false;
        }

        // Выйти из системы — сбрасываем текущего пользователя
        public void Logout()
        {
            CurrentUser = null;
        }

        // Проверить, вошёл ли пользователь в систему
        public bool IsLoggedIn()
        {
            return CurrentUser != null;
        }

        // Проверить, является ли текущий пользователь администратором
        public bool IsAdmin()
        {
            return CurrentUser?.Role == "Admin";
        }

        // Проверить, является ли текущий пользователь студентом
        public bool IsStudent()
        {
            return CurrentUser?.Role == "Student";
        }

        // Зарегистрировать нового пользователя
        // Возвращает null если логин уже занят
        public async Task<User?> RegisterAsync(string username, string password, string role = "Student")
        {
            // Проверяем, нет ли уже такого логина
            bool exists = await _context.Users.AnyAsync(u => u.Username == username);
            if (exists)
                return null; // Логин уже занят

            // Создаём нового пользователя с захешированным паролем
            var newUser = new User
            {
                Username = username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Role = role,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            return newUser;
        }

        // Сменить пароль текущего пользователя
        // Возвращает true если смена прошла успешно
        public async Task<bool> ChangePasswordAsync(string oldPassword, string newPassword)
        {
            if (CurrentUser == null)
                return false;

            // Сначала проверяем старый пароль
            bool oldPasswordCorrect = BCrypt.Net.BCrypt.Verify(oldPassword, CurrentUser.PasswordHash);
            if (!oldPasswordCorrect)
                return false;

            // Сохраняем новый пароль (хешируем)
            CurrentUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
