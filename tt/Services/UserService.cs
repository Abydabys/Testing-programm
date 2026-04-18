using Microsoft.EntityFrameworkCore;
using tt.Data;
using tt.Models;

namespace tt.Services
{
    public interface IUserService
    {
        Task<User> GetUserByIdAsync(int id);
        Task<User> GetUserByUsernameAsync(string username);
        Task<bool> CreateUserAsync(User user);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> DeactivateUserAsync(int userId);
    }

    public class UserService : IUserService
    {
        private readonly TestingDbContext _dbContext;

        public UserService(TestingDbContext dbContext)
        {
            // TODO: Store the dbContext parameter in the _dbContext field.
        }

        public async Task<User> GetUserByIdAsync(int id)
        {
            // TODO: Use _dbContext.Users.FindAsync(id) to look up the user by primary key.
            // TODO: Return the result (or null if not found).
            throw new NotImplementedException();
        }

        public async Task<User> GetUserByUsernameAsync(string username)
        {
            // TODO: Query _dbContext.Users to find the first user where Username == username.
            // TODO: Return the result, or null if no match is found.
            throw new NotImplementedException();
        }

        public async Task<bool> CreateUserAsync(User user)
        {
            // TODO: Wrap in a try-catch block.
            // TODO: In the try block, add the user to _dbContext.Users.
            // TODO: Call SaveChangesAsync to persist the new user.
            // TODO: Return true on success.
            // TODO: In the catch block, return false.
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateUserAsync(User user)
        {
            // TODO: Wrap in a try-catch block.
            // TODO: In the try block, call _dbContext.Users.Update(user).
            // TODO: Call SaveChangesAsync to persist the changes.
            // TODO: Return true on success.
            // TODO: In the catch block, return false.
            throw new NotImplementedException();
        }

        public async Task<bool> DeactivateUserAsync(int userId)
        {
            // TODO: Wrap in a try-catch block.
            // TODO: In the try block, find the user by userId using FindAsync. If null, return false.
            // TODO: Set user.IsActive to false.
            // TODO: Call SaveChangesAsync to persist the change.
            // TODO: Return true on success.
            // TODO: In the catch block, return false.
            throw new NotImplementedException();
        }
    }
}
