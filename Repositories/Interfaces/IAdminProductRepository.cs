using DoAnNhom08LTW.Models;

namespace DoAnNhom08LTW.Repositories.Interfaces
{
    public interface IAdminProductRepository
    {
        Task<IEnumerable<SanPham>> GetAllAsync(
            string? keyword = null,
            int? categoryId = null,
            bool? dangBan = null,
            string? stockFilter = null);

        Task<SanPham?> GetByIdAsync(int id);
        Task<int> CountAllAsync();
        Task<int> CountActiveAsync();
        Task<int> CountLowStockAsync(int threshold = 10);
        Task<IEnumerable<SanPham>> GetLowStockProductsAsync(int threshold = 10, int take = 5);
        Task AddAsync(SanPham sanPham);
        Task UpdateAsync(SanPham sanPham);
        Task SaveAsync();
    }
}
