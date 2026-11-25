using DoAnNhom08LTW.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAnNhom08LTW.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")] // Chỉ tài khoản có VaiTro = 'Admin' mới được truy cập
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrderController> _logger;

        public OrderController(
            IOrderService orderService,
            ILogger<OrderController> logger)
        {
            _orderService = orderService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? status, string? keyword)
        {
            ViewBag.Status = status;
            ViewBag.Keyword = keyword;

            var orders = await _orderService.GetAllOrdersAsync(status, keyword);
            return View(orders);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetOrderDetailAsync(id);
            if (order == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy đơn hàng yêu cầu.";
                return RedirectToAction(nameof(Index));
            }

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirm(int id, string? returnUrl = null)
        {
            var (success, message) = await _orderService.ConfirmOrderAsync(id);
            if (success)
            {
                _logger.LogInformation("Admin đã xác nhận đơn hàng {OrderId}.", id);
                TempData["SuccessMessage"] = message;
            }
            else
            {
                TempData["ErrorMessage"] = message;
            }

            return RedirectBack(returnUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id, string? reason = null, string? returnUrl = null)
        {
            var (success, message) = await _orderService.CancelOrderAsync(id, reason);
            if (success)
            {
                _logger.LogInformation("Admin đã hủy đơn hàng {OrderId}. Lý do: {Reason}", id, reason ?? "Không có");
                TempData["SuccessMessage"] = message;
            }
            else
            {
                TempData["ErrorMessage"] = message;
            }

            return RedirectBack(returnUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string newStatus, string? returnUrl = null)
        {
            var (success, message) = await _orderService.UpdateDeliveryStatusAsync(id, newStatus);
            if (success)
            {
                _logger.LogInformation("Admin đã cập nhật trạng thái đơn hàng {OrderId} -> {Status}.", id, newStatus);
                TempData["SuccessMessage"] = message;
            }
            else
            {
                TempData["ErrorMessage"] = message;
            }

            return RedirectBack(returnUrl);
        }

        private IActionResult RedirectBack(string? returnUrl)
        {
            if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
