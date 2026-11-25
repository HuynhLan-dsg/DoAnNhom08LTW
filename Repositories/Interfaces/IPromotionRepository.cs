using DoAnNhom08LTW.Models;

namespace DoAnNhom08LTW.Repositories.Interfaces
{
    public interface IPromotionRepository
    {
        Task<IEnumerable<KhuyenMai>> GetAllAsync(string? keyword = null, bool? trangThai = null);
        Task<IEnumerable<KhuyenMai>> GetActiveAsync();
        Task<KhuyenMai?> GetByIdAsync(int id);
        Task<int> CountExpiringSoonAsync(int days = 7);
        Task AddAsync(KhuyenMai khuyenMai);
        Task UpdateAsync(KhuyenMai khuyenMai);
        Task DetachProductsAsync(int promotionId);
        Task DeleteAsync(KhuyenMai khuyenMai);
        Task SaveAsync();
    }
}
