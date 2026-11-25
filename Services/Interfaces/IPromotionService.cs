using DoAnNhom08LTW.Models;
using DoAnNhom08LTW.Models.ViewModels.Admin;

namespace DoAnNhom08LTW.Services.Interfaces
{
    public interface IPromotionService
    {
        Task<IEnumerable<KhuyenMai>> GetAllAsync(string? keyword = null, bool? trangThai = null);
        Task<IEnumerable<KhuyenMai>> GetActiveAsync();
        Task<KhuyenMai?> GetByIdAsync(int id);
        Task<PromotionFormViewModel?> GetFormByIdAsync(int id);
        Task<(bool success, string message)> CreateAsync(PromotionFormViewModel model);
        Task<(bool success, string message)> UpdateAsync(PromotionFormViewModel model);
        Task<(bool success, string message)> DeleteAsync(int id);
        Task<(bool success, string message)> ToggleStatusAsync(int id);
    }
}
