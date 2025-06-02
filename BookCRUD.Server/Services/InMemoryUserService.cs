using BookCRUD.Server.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookCRUD.Server.Services
{
    public class InMemoryUserService
    {
        private static readonly List<User> _users = new List<User>();

        public Task<User> AddUserAsync(string username, string password, string role)
        {
            if (_users.Any(u => u.Username == username))
            {
                // User already exists
                return Task.FromResult<User>(null);
            }

            // Placeholder for password hashing
            var passwordHash = password + "_hashed";

            var user = new User
            {
                Id = Guid.NewGuid(),
                Username = username,
                PasswordHash = passwordHash,
                Role = role
            };

            _users.Add(user);
            return Task.FromResult(user);
        }

        public Task<User> FindUserByUsernameAsync(string username)
        {
            var user = _users.FirstOrDefault(u => u.Username == username);
            return Task.FromResult(user);
        }

        public async Task<bool> ValidateCredentialsAsync(string username, string password)
        {
            var user = await FindUserByUsernameAsync(username);
            if (user != null)
            {
                // Placeholder for password hashing comparison
                var passwordHashToCompare = password + "_hashed";
                return user.PasswordHash == passwordHashToCompare;
            }
            return false;
        }
    }
}
