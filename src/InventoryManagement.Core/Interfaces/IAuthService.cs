using InventoryManagement.Core.Entities;

namespace InventoryManagement.Core.Interfaces;

public interface IAuthService
{
    Task<(bool success, string token)> AuthenticateAsync(string username, string password);
    Task<bool> RegisterUserAsync(User user, string password);
    Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
    Task<bool> ResetPasswordAsync(string email);
    Task<bool> ValidateTokenAsync(string token);
    Task<User?> GetCurrentUserAsync();
    Task<IEnumerable<string>> GetUserRolesAsync(Guid userId);
}
