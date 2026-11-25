using DoAnNhom08LTW.Models.ViewModels.Admin;
using DoAnNhom08LTW.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAnNhom08LTW.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")] // Chỉ tài khoản có VaiTro = 'Admin' mới được truy cập
    public class PromotionController : Controller
    {
        private readonly IPromotionService _promotionService;
        private readonly ILogger<PromotionController> _logger;

        public PromotionController(
            IPromotionService promotionService,
            ILogger<PromotionController> logger)
        {
            _promotionService = promotionService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? keyword, bool? trangThai)
        {
            ViewBag.Keyword = keyword;
            ViewBag.TrangThai = trangThai;
            var promotions = await _promotionService.GetAllAsync(keyword, trangThai);
            return View(promotions);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new PromotionFormViewModel
            {
                LoaiGiam = PromotionHelper.PhanTram,
                NgayBatDau = DateTime.Today,
                NgayKetThuc = DateTime.Today.AddDays(30),
                TrangThai = true
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PromotionFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var (success, message) = await _promotionService.CreateAsync(model);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, message);
                return View(model);
            }

            _logger.LogInformation("Admin đã thêm khuyến mãi mới: {PromotionName}.", model.TenKhuyenMai);
            TempData["SuccessMessage"] = message;
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _promotionService.GetFormByIdAsync(id);
            if (model == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy chương trình khuyến mãi.";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PromotionFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var (success, message) = await _promotionService.UpdateAsync(model);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, message);
                return View(model);
            }

            _logger.LogInformation("Admin đã cập nhật khuyến mãi {PromotionId}.", model.Id);
            TempData["SuccessMessage"] = message;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, message) = await _promotionService.DeleteAsync(id);
            if (success)
            {
                _logger.LogInformation("Admin đã xóa khuyến mãi ID {PromotionId}.", id);
                TempData["SuccessMessage"] = message;
            }
            else
            {
                TempData["ErrorMessage"] = message;
            }

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var (success, message) = await _promotionService.ToggleStatusAsync(id);
            if (success)
            {
                _logger.LogInformation("Admin đã thay trạng thái khuyến mãi {PromotionId}.", id);
                TempData["SuccessMessage"] = message;
            }
            else
            {
                TempData["ErrorMessage"] = message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
