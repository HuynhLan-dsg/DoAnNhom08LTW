using DoAnNhom08LTW.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAnNhom08LTW.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")] // Chỉ tài khoản có VaiTro = 'Admin' mới được truy cập
    //Cơ chế hoạt động:Nếu chưa đăng nhập mà gõ /Admin/Dashboard ==> Tự động chuyển sang /Account/Login.
    //Nếu đăng nhập bằng tài khoản khach01 @phonestore.example(có VaiTro = 'KhachHang') mà cố tình gõ /Admin/Dashboard $\rightarrow$ Tự động chặn lại và chuyển sang /Account/AccessDenied.
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(
            IDashboardService dashboardService,
            ILogger<DashboardController> logger)
        {
            _dashboardService = dashboardService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int? year)
        {
            var model = await _dashboardService.GetDashboardDataAsync(year);
            _logger.LogInformation("Admin truy cập Dashboard thống kê năm {Year}.", model.NamThongKe);
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> RevenueByMonth(int year)
        {
            var targetYear = year > 2000 ? year : DateTime.Now.Year;
            var data = await _dashboardService.GetMonthlyRevenueAsync(targetYear);
            return Json(new { year = targetYear, items = data });
        }
    }
}
