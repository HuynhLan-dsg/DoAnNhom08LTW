using DoAnNhom08LTW.Models;
using DoAnNhom08LTW.Models.ViewModels.Admin;

namespace DoAnNhom08LTW.Services.Interfaces
{
    public interface IAdminProductService
    {
        Task<IEnumerable<SanPham>> GetAllAsync(
            string? keyword = null,
            int? categoryId = null,
            bool? dangBan = null,
            string? stockFilter = null);

        Task<SanPham?> GetByIdAsync(int id);
        Task<ProductFormViewModel?> GetFormByIdAsync(int id);
        Task<(bool success, string message)> CreateAsync(ProductFormViewModel model, string webRootPath);
        Task<(bool success, string message)> UpdateAsync(ProductFormViewModel model, string webRootPath);
        Task<(bool success, string message)> SoftDeleteAsync(int id);
        Task<(bool success, string message)> RestoreAsync(int id);
        Task<(bool success, string message)> UpdateStockAsync(int id, int newQuantity);
    }
}
