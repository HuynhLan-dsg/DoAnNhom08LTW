using DoAnNhom08LTW.Models.ViewModels.Admin;
using DoAnNhom08LTW.Repositories.Interfaces;
using DoAnNhom08LTW.Services.Interfaces;

namespace DoAnNhom08LTW.Services.Implementations
{
    public class DashboardService : IDashboardService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IAdminProductRepository _productRepository;
        private readonly IAccountRepository _accountRepository;

        public DashboardService(
            IOrderRepository orderRepository,
            IAdminProductRepository productRepository,
            IAccountRepository accountRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _accountRepository = accountRepository;
        }

        public async Task<DashboardViewModel> GetDashboardDataAsync(int? year = null)
        {
            var availableYears = await _orderRepository.GetAvailableOrderYearsAsync();
            var selectedYear = year.HasValue && year.Value > 2000
                ? year.Value
                : (availableYears.FirstOrDefault() > 0 ? availableYears.First() : DateTime.Now.Year);

            if (!availableYears.Contains(selectedYear))
            {
                availableYears.Add(selectedYear);
                availableYears = availableYears.OrderByDescending(y => y).ToList();
            }

            var accounts = await _accountRepository.GetAllAsync();
            var totalCustomers = accounts.Count(a => string.Equals(a.VaiTro, "KhachHang", StringComparison.OrdinalIgnoreCase));

            var model = new DashboardViewModel
            {
                TongDoanhThu = await _orderRepository.GetTotalRevenueAsync(),
                TongDonHang = await _orderRepository.CountAsync(),
                DonHangChoXacNhan = await _orderRepository.CountByStatusAsync(OrderStatusHelper.ChoXacNhan),
                TongSanPham = await _productRepository.CountAllAsync(),
                SanPhamDangBan = await _productRepository.CountActiveAsync(),
                SanPhamSapHetHang = await _productRepository.CountLowStockAsync(10),
                TongKhachHang = totalCustomers,
                NamThongKe = selectedYear,
                DanhSachNam = availableYears,
                DoanhThuTheoThang = await _orderRepository.GetMonthlyRevenueAsync(selectedYear),
                DonHangGanDay = (await _orderRepository.GetRecentOrdersAsync(5)).ToList()
            };

            return model;
        }

        public async Task<List<MonthlyRevenueItem>> GetMonthlyRevenueAsync(int year)
        {
            return await _orderRepository.GetMonthlyRevenueAsync(year);
        }
    }
}
