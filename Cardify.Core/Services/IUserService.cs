using Cardify.Core.Models;

namespace Cardify.Core.Services
{
    public interface IUserService
    {
        Task<bool> RegisterUserAsync(UserCreateDto dto);
        Task<object?> LoginUserAsync(UserLoginDto dto);
        Task<bool> LogoutUserAsync(int userId);
        Task<bool> ChangePasswordAsync(int userId, PasswordChangeDto dto);
        Task<User?> GetUserByIdAsync(int userId);
    }
} 