using DoAnNhom08LTW.Models.ViewModels.Admin;
using DoAnNhom08LTW.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAnNhom08LTW.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")] // Chỉ tài khoản có VaiTro = 'Admin' mới được truy cập
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(
            ICategoryService categoryService,
            IWebHostEnvironment env,
            ILogger<CategoryController> logger)
        {
            _categoryService = categoryService;
            _env = env;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string? keyword, bool? isActive)
        {
            ViewBag.Keyword = keyword;
            ViewBag.IsActive = isActive;
            var categories = await _categoryService.GetAllAsync(keyword, isActive);
            return View(categories);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View(new CategoryFormViewModel { DangHoatDong = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var (success, message) = await _categoryService.CreateAsync(model, _env.WebRootPath);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, message);
                return View(model);
            }

            _logger.LogInformation("Admin đã thêm danh mục mới: {CategoryName}.", model.TenDanhMuc);
            TempData["SuccessMessage"] = message;
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _categoryService.GetFormByIdAsync(id);
            if (model == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy danh mục yêu cầu.";
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var (success, message) = await _categoryService.UpdateAsync(model, _env.WebRootPath);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, message);
                return View(model);
            }

            _logger.LogInformation("Admin đã cập nhật danh mục ID {CategoryId}.", model.Id);
            TempData["SuccessMessage"] = message;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var (success, message) = await _categoryService.DeleteAsync(id);
            if (success)
            {
                _logger.LogInformation("Admin đã xóa danh mục ID {CategoryId}.", id);
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
            var (success, message) = await _categoryService.ToggleStatusAsync(id);
            if (success)
            {
                _logger.LogInformation("Admin đã đổi trạng thái danh mục ID {CategoryId}.", id);
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
