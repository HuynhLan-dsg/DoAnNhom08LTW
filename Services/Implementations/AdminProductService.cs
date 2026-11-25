using DoAnNhom08LTW.Models;
using DoAnNhom08LTW.Models.ViewModels.Admin;
using DoAnNhom08LTW.Repositories.Interfaces;
using DoAnNhom08LTW.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace DoAnNhom08LTW.Services.Implementations
{
    public class AdminProductService : IAdminProductService
    {
        private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
        private const long MaxImageBytes = 5 * 1024 * 1024;

        private readonly IAdminProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IPromotionRepository _promotionRepository;

        public AdminProductService(
            IAdminProductRepository productRepository,
            ICategoryRepository categoryRepository,
            IPromotionRepository promotionRepository)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _promotionRepository = promotionRepository;
        }

        public async Task<IEnumerable<SanPham>> GetAllAsync(
            string? keyword = null,
            int? categoryId = null,
            bool? dangBan = null,
            string? stockFilter = null)
        {
            return await _productRepository.GetAllAsync(keyword, categoryId, dangBan, stockFilter);
        }

        public async Task<SanPham?> GetByIdAsync(int id)
        {
            return await _productRepository.GetByIdAsync(id);
        }

        public async Task<ProductFormViewModel?> GetFormByIdAsync(int id)
        {
            var product = await _productRepository.GetByIdAsync(id);
            if (product == null) return null;

            return new ProductFormViewModel
            {
                Id = product.Id,
                MaSanPham = product.MaSanPham,
                TenSanPham = product.TenSanPham,
                MaDanhMucId = product.MaDanhMucId,
                MaKhuyenMaiId = product.MaKhuyenMaiId,
                ThuongHieu = product.ThuongHieu,
                GiaBan = product.GiaBan,
                SoLuongTon = product.SoLuongTon,
                HinhAnh = product.HinhAnh,
                MoTa = product.MoTa,
                DangBan = product.DangBan,
                NgayTao = product.NgayTao
            };
        }

        public async Task<(bool success, string message)> CreateAsync(ProductFormViewModel model, string webRootPath)
        {
            var category = await _categoryRepository.GetByIdAsync(model.MaDanhMucId);
            if (category == null)
            {
                return (false, "Danh mục được chọn không tồn tại.");
            }

            if (model.MaKhuyenMaiId.HasValue)
            {
                var promo = await _promotionRepository.GetByIdAsync(model.MaKhuyenMaiId.Value);
                if (promo == null)
                {
                    return (false, "Chương trình khuyến mãi được chọn không tồn tại.");
                }
            }

            var (uploadOk, imagePath, uploadError) = await SaveImageIfProvidedAsync(model.HinhAnhFile, model.HinhAnh, webRootPath);
            if (!uploadOk)
            {
                return (false, uploadError);
            }

            var entity = new SanPham
            {
                TenSanPham = model.TenSanPham.Trim(),
                MaDanhMucId = model.MaDanhMucId,
                MaKhuyenMaiId = model.MaKhuyenMaiId,
                ThuongHieu = model.ThuongHieu.Trim(),
                GiaBan = model.GiaBan,
                SoLuongTon = model.SoLuongTon,
                HinhAnh = imagePath,
                MoTa = string.IsNullOrWhiteSpace(model.MoTa) ? null : model.MoTa.Trim(),
                DangBan = model.DangBan,
                NgayTao = DateTime.Now
            };

            await _productRepository.AddAsync(entity);
            await _productRepository.SaveAsync();

            return (true, $"Thêm sản phẩm \"{entity.TenSanPham}\" thành công!");
        }

        public async Task<(bool success, string message)> UpdateAsync(ProductFormViewModel model, string webRootPath)
        {
            var existing = await _productRepository.GetByIdAsync(model.Id);
            if (existing == null)
            {
                return (false, "Không tìm thấy sản phẩm cần cập nhật.");
            }

            var category = await _categoryRepository.GetByIdAsync(model.MaDanhMucId);
            if (category == null)
            {
                return (false, "Danh mục được chọn không tồn tại.");
            }

            if (model.MaKhuyenMaiId.HasValue)
            {
                var promo = await _promotionRepository.GetByIdAsync(model.MaKhuyenMaiId.Value);
                if (promo == null)
                {
                    return (false, "Chương trình khuyến mãi được chọn không tồn tại.");
                }
            }

            var (uploadOk, imagePath, uploadError) = await SaveImageIfProvidedAsync(
                model.HinhAnhFile,
                model.HinhAnh ?? existing.HinhAnh,
                webRootPath);

            if (!uploadOk)
            {
                return (false, uploadError);
            }

            existing.TenSanPham = model.TenSanPham.Trim();
            existing.MaDanhMucId = model.MaDanhMucId;
            existing.MaKhuyenMaiId = model.MaKhuyenMaiId;
            existing.ThuongHieu = model.ThuongHieu.Trim();
            existing.GiaBan = model.GiaBan;
            existing.SoLuongTon = model.SoLuongTon;
            existing.HinhAnh = imagePath;
            existing.MoTa = string.IsNullOrWhiteSpace(model.MoTa) ? null : model.MoTa.Trim();
            existing.DangBan = model.DangBan;

            await _productRepository.UpdateAsync(existing);
            return (true, $"Cập nhật sản phẩm \"{existing.TenSanPham}\" thành công!");
        }

        public async Task<(bool success, string message)> SoftDeleteAsync(int id)
        {
            var existing = await _productRepository.GetByIdAsync(id);
            if (existing == null)
            {
                return (false, "Không tìm thấy sản phẩm.");
            }

            if (!existing.DangBan)
            {
                return (false, $"Sản phẩm \"{existing.TenSanPham}\" đã ở trạng thái ngừng bán.");
            }

            existing.DangBan = false;
            await _productRepository.UpdateAsync(existing);
            return (true, $"Đã xóa mềm (ngừng bán) sản phẩm \"{existing.TenSanPham}\" thành công!");
        }

        public async Task<(bool success, string message)> RestoreAsync(int id)
        {
            var existing = await _productRepository.GetByIdAsync(id);
            if (existing == null)
            {
                return (false, "Không tìm thấy sản phẩm.");
            }

            existing.DangBan = true;
            await _productRepository.UpdateAsync(existing);
            return (true, $"Đã mở bán lại sản phẩm \"{existing.TenSanPham}\" thành công!");
        }

        public async Task<(bool success, string message)> UpdateStockAsync(int id, int newQuantity)
        {
            if (newQuantity < 0)
            {
                return (false, "Số lượng tồn kho không được nhỏ hơn 0.");
            }

            var existing = await _productRepository.GetByIdAsync(id);
            if (existing == null)
            {
                return (false, "Không tìm thấy sản phẩm.");
            }

            existing.SoLuongTon = newQuantity;
            await _productRepository.UpdateAsync(existing);
            return (true, $"Đã cập nhật tồn kho sản phẩm \"{existing.TenSanPham}\" thành {newQuantity} sản phẩm!");
        }

        private static async Task<(bool ok, string? path, string error)> SaveImageIfProvidedAsync(
            IFormFile? file,
            string? fallbackPath,
            string webRootPath)
        {
            if (file == null || file.Length == 0)
            {
                return (true, string.IsNullOrWhiteSpace(fallbackPath) ? null : fallbackPath.Trim(), string.Empty);
            }

            if (file.Length > MaxImageBytes)
            {
                return (false, null, "Kích thước hình ảnh vượt quá giới hạn 5MB.");
            }

            var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedImageExtensions.Contains(ext))
            {
                return (false, null, "Chỉ hỗ trợ định dạng ảnh: .jpg, .jpeg, .png, .webp, .gif.");
            }

            var folder = Path.Combine(webRootPath, "images", "sanpham");
            Directory.CreateDirectory(folder);

            var fileName = $"sp_{Guid.NewGuid():N}{ext}";
            var fullPath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return (true, $"/images/sanpham/{fileName}", string.Empty);
        }
    }
}
