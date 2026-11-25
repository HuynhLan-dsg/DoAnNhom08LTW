using DoAnNhom08LTW.Models;

namespace DoAnNhom08LTW.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<IEnumerable<DanhMuc>> GetAllAsync(string? keyword = null, bool? isActive = null);
        Task<IEnumerable<DanhMuc>> GetActiveAsync();
        Task<DanhMuc?> GetByIdAsync(int id);
        Task<bool> ExistsByNameAsync(string tenDanhMuc, int? excludeId = null);
        Task<bool> HasProductsAsync(int id);
        Task AddAsync(DanhMuc danhMuc);
        Task UpdateAsync(DanhMuc danhMuc);
        Task DeleteAsync(DanhMuc danhMuc);
        Task SaveAsync();
    }
}
