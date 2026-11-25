using DoAnNhom08LTW.Models.ViewModels.Admin;
using DoAnNhom08LTW.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DoAnNhom08LTW.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")] // Chỉ tài khoản có VaiTro = 'Admin' mới được truy cập
    public class ProductController : Controller
    {
        private readonly IAdminProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IPromotionService _promotionService;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<ProductController> _logger;

        public ProductController(
            IAdminProductService productService,
            ICategoryService categoryService,
            IPromotionService promotionService,
            IWebHostEnvironment env,
            ILogger<ProductController> logger)
        {
            _productService = productService;
            _categoryService = categoryService;
            _promotionService = promotionService;
            _env = env;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(
            string? keyword,
            int? categoryId,
            bool? dangBan,
            string? stockFilter)
        {
            ViewBag.Keyword = keyword;
            ViewBag.CategoryId = categoryId;
            ViewBag.DangBan = dangBan;
            ViewBag.StockFilter = stockFilter;

            var categories = await _categoryService.GetAllAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "TenDanhMuc", categoryId);

            var products = await _productService.GetAllAsync(keyword, categoryId, dangBan, stockFilter);
            return View(products);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateFormSelectListsAsync();
            return View(new ProductFormViewModel { DangBan = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateFormSelectListsAsync(model.MaDanhMucId, model.MaKhuyenMaiId);
                return View(model);
            }

            var (success, message) = await _productService.CreateAsync(model, _env.WebRootPath);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, message);
                await PopulateFormSelectListsAsync(model.MaDanhMucId, model.MaKhuyenMaiId);
                return View(model);
            }

            _logger.LogInformation("Admin đã thêm sản phẩm mới: {ProductName}.", model.TenSanPham);
            TempData["SuccessMessage"] = message;
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var model = await _productService.GetFormByIdAsync(id);
            if (model == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy sản phẩm yêu cầu.";
                return RedirectToAction(nameof(Index));
            }

            await PopulateFormSelectListsAsync(model.MaDanhMucId, model.MaKhuyenMaiId);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await PopulateFormSelectListsAsync(model.MaDanhMucId, model.MaKhuyenMaiId);
                return View(model);
            }

            var (success, message) = await _productService.UpdateAsync(model, _env.WebRootPath);
            if (!success)
            {
                ModelState.AddModelError(string.Empty, message);
                await PopulateFormSelectListsAsync(model.MaDanhMucId, model.MaKhuyenMaiId);
                return View(model);
            }

            _logger.LogInformation("Admin đã cập nhật sản phẩm {ProductId}.", model.Id);
            TempData["SuccessMessage"] = message;
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SoftDelete(int id)
        {
            var (success, message) = await _productService.SoftDeleteAsync(id);
            if (success)
            {
                _logger.LogInformation("Admin đã xóa mềm sản phẩm {ProductId}.", id);
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
        public async Task<IActionResult> Restore(int id)
        {
            var (success, message) = await _productService.RestoreAsync(id);
            if (success)
            {
                _logger.LogInformation("Admin đã mở bán lại sản phẩm {ProductId}.", id);
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
        public async Task<IActionResult> UpdateStock(int id, int soLuongTon)
        {
            var (success, message) = await _productService.UpdateStockAsync(id, soLuongTon);
            if (success)
            {
                _logger.LogInformation("Admin đã cập nhật tồn kho sản phẩm {ProductId} -> {Qty}.", id, soLuongTon);
                TempData["SuccessMessage"] = message;
            }
            else
            {
                TempData["ErrorMessage"] = message;
            }

            return RedirectToAction(nameof(Index));
        }

        private async Task PopulateFormSelectListsAsync(int? selectedCategoryId = null, int? selectedPromotionId = null)
        {
            var categories = await _categoryService.GetAllAsync();
            var promotions = await _promotionService.GetAllAsync();

            ViewBag.Categories = new SelectList(categories, "Id", "TenDanhMuc", selectedCategoryId);
            ViewBag.Promotions = promotions
                .Select(p => new SelectListItem
                {
                    Value = p.Id.ToString(),
                    Text = $"{p.MaKhuyenMai} - {p.TenKhuyenMai} ({PromotionHelper.FormatDiscount(p.LoaiGiam, p.GiaTriGiam)})",
                    Selected = selectedPromotionId.HasValue && p.Id == selectedPromotionId.Value
                })
                .ToList();
        }
    }
}
