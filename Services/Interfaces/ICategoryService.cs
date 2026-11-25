using DoAnNhom08LTW.Models;
using DoAnNhom08LTW.Models.ViewModels.Admin;

namespace DoAnNhom08LTW.Services.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<DanhMuc>> GetAllAsync(string? keyword = null, bool? isActive = null);
        Task<IEnumerable<DanhMuc>> GetActiveAsync();
        Task<DanhMuc?> GetByIdAsync(int id);
        Task<CategoryFormViewModel?> GetFormByIdAsync(int id);
        Task<(bool success, string message)> CreateAsync(CategoryFormViewModel model, string webRootPath);
        Task<(bool success, string message)> UpdateAsync(CategoryFormViewModel model, string webRootPath);
        Task<(bool success, string message)> DeleteAsync(int id);
        Task<(bool success, string message)> ToggleStatusAsync(int id);
    }
}
