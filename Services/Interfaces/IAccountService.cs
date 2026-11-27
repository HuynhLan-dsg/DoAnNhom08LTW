using DoAnNhom08LTW.Models;
using DoAnNhom08LTW.Models.ViewModels;

namespace DoAnNhom08LTW.Services.Interfaces
{
    public interface IAccountService
    {
        Task<bool> RegisterAsync(RegisterViewModel model);
        Task<TaiKhoan?> LoginAsync(LoginViewModel model);
        Task<ProfileViewModel?> GetProfileAsync(int id);
        Task<bool> UpdateProfileAsync(ProfileViewModel model);
        Task<(bool success, string text)> ChangePasswordAsync(int id, ChangePasswordViewModel model);
        Task<IEnumerable<TaiKhoan>> GetAllAccountsAsync();
        Task<(bool success, string text)> ToggleAccountStatusAsync(int id, int currentAdminId);
        Task<(bool success, string text)> ChangeAccountRoleAsync(int id, string newRole, int currentAdminId);
        Task<TaiKhoan?> GetByIdAsync(int id);
        Task<bool> IsEmailExistsAsync(string email);
    }
}
