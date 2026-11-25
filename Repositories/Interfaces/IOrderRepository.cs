using DoAnNhom08LTW.Models;
using DoAnNhom08LTW.Models.ViewModels.Admin;

namespace DoAnNhom08LTW.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task<IEnumerable<DonHang>> GetAllAsync(string? status = null, string? keyword = null);
        Task<IEnumerable<DonHang>> GetByUserIdAsync(int userId);
        Task<DonHang?> GetByIdAsync(int id);
        Task<DonHang?> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<DonHang>> GetRecentOrdersAsync(int count = 5);
        Task<int> CountAsync();
        Task<int> CountByStatusAsync(string status);
        Task<decimal> GetTotalRevenueAsync();
        Task<List<MonthlyRevenueItem>> GetMonthlyRevenueAsync(int year);
        Task<List<int>> GetAvailableOrderYearsAsync();
        Task UpdateAsync(DonHang donHang);
        Task SaveAsync();
    }
}
