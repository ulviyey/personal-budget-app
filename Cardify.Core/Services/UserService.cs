using Cardify.Core.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Cardify.Core.Services
{
    public class UserService : IUserService
    {
        private readonly CardifyDbContext _context;

        public UserService(CardifyDbContext context)
        {
            _context = context;
        }

        public async Task<bool> RegisterUserAsync(UserCreateDto dto)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return false;

            if (await _context.Users.AnyAsync(u => u.Username == dto.Username))
                return false;
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return false;

            // Hash password
            string passwordHash = HashPassword(dto.Password);

            var user = new User
            {
                Username = dto.Username,
                Email = dto.Email,
                Name = dto.Name,
                PasswordHash = passwordHash,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<object?> LoginUserAsync(UserLoginDto dto)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(dto.UsernameOrEmail) || string.IsNullOrWhiteSpace(dto.Password))
                return null;

            // Find user by username or email
            var user = await _context.Users.FirstOrDefaultAsync(u => 
                u.Username == dto.UsernameOrEmail || u.Email == dto.UsernameOrEmail);

            if (user == null)
                return null;

            // Verify password
            string hashedPassword = HashPassword(dto.Password);
            if (user.PasswordHash != hashedPassword)
                return null;

            if (!user.IsActive)
                return null;

            // Update last login time
            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return new
            {
                message = "Login successful.",
                user = new
                {
                    id = user.Id,
                    username = user.Username,
                    email = user.Email,
                    name = user.Name
                }
            };
        }

        public async Task<bool> LogoutUserAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return false;

            user.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ChangePasswordAsync(int userId, PasswordChangeDto dto)
        {
            // Validate input
            if (string.IsNullOrWhiteSpace(dto.CurrentPassword) || string.IsNullOrWhiteSpace(dto.NewPassword))
                return false;

            if (dto.NewPassword.Length < 6)
                return false;

            // Find user
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return false;

            // Verify current password
            string currentPasswordHash = HashPassword(dto.CurrentPassword);
            if (user.PasswordHash != currentPasswordHash)
                return false;

            // Hash and update new password
            string newPasswordHash = HashPassword(dto.NewPassword);
            user.PasswordHash = newPasswordHash;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.Users.FindAsync(userId);
        }

        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
} 