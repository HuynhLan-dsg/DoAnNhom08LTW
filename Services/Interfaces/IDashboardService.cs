using DoAnNhom08LTW.Models.ViewModels.Admin;

namespace DoAnNhom08LTW.Services.Interfaces
{
    public interface IDashboardService
    {
        Task<DashboardViewModel> GetDashboardDataAsync(int? year = null);
        Task<List<MonthlyRevenueItem>> GetMonthlyRevenueAsync(int year);
    }
}
