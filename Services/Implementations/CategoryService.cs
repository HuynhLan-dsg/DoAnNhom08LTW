using DoAnNhom08LTW.Models;
using DoAnNhom08LTW.Models.ViewModels.Admin;
using DoAnNhom08LTW.Repositories.Interfaces;
using DoAnNhom08LTW.Services.Interfaces;
using Microsoft.AspNetCore.Http;

namespace DoAnNhom08LTW.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private static readonly string[] AllowedImageExtensions = { ".jpg", ".jpeg", ".png", ".webp", ".gif" };
        private const long MaxImageBytes = 5 * 1024 * 1024;

        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<DanhMuc>> GetAllAsync(string? keyword = null, bool? isActive = null)
        {
            return await _categoryRepository.GetAllAsync(keyword, isActive);
        }

        public async Task<IEnumerable<DanhMuc>> GetActiveAsync()
        {
            return await _categoryRepository.GetActiveAsync();
        }

        public async Task<DanhMuc?> GetByIdAsync(int id)
        {
            return await _categoryRepository.GetByIdAsync(id);
        }

        public async Task<CategoryFormViewModel?> GetFormByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return null;

            return new CategoryFormViewModel
            {
                Id = category.Id,
                MaDanhMuc = category.MaDanhMuc,
                TenDanhMuc = category.TenDanhMuc,
                HinhAnh = category.HinhAnh,
                DangHoatDong = category.DangHoatDong
            };
        }

        public async Task<(bool success, string message)> CreateAsync(CategoryFormViewModel model, string webRootPath)
        {
            var tenDanhMuc = model.TenDanhMuc.Trim();
            if (await _categoryRepository.ExistsByNameAsync(tenDanhMuc))
            {
                return (false, $"Danh mục \"{tenDanhMuc}\" đã tồn tại trong hệ thống.");
            }

            var (uploadOk, imagePath, uploadError) = await SaveImageIfProvidedAsync(model.HinhAnhFile, model.HinhAnh, webRootPath);
            if (!uploadOk)
            {
                return (false, uploadError);
            }

            var entity = new DanhMuc
            {
                TenDanhMuc = tenDanhMuc,
                HinhAnh = imagePath,
                DangHoatDong = model.DangHoatDong
            };

            await _categoryRepository.AddAsync(entity);
            await _categoryRepository.SaveAsync();

            return (true, $"Thêm danh mục \"{entity.TenDanhMuc}\" thành công!");
        }

        public async Task<(bool success, string message)> UpdateAsync(CategoryFormViewModel model, string webRootPath)
        {
            var existing = await _categoryRepository.GetByIdAsync(model.Id);
            if (existing == null)
            {
                return (false, "Không tìm thấy danh mục cần cập nhật.");
            }

            var tenDanhMuc = model.TenDanhMuc.Trim();
            if (await _categoryRepository.ExistsByNameAsync(tenDanhMuc, model.Id))
            {
                return (false, $"Tên danh mục \"{tenDanhMuc}\" đã được sử dụng bởi danh mục khác.");
            }

            var (uploadOk, imagePath, uploadError) = await SaveImageIfProvidedAsync(model.HinhAnhFile, model.HinhAnh ?? existing.HinhAnh, webRootPath);
            if (!uploadOk)
            {
                return (false, uploadError);
            }

            existing.TenDanhMuc = tenDanhMuc;
            existing.HinhAnh = imagePath;
            existing.DangHoatDong = model.DangHoatDong;

            await _categoryRepository.UpdateAsync(existing);

            return (true, $"Cập nhật danh mục \"{existing.TenDanhMuc}\" thành công!");
        }

        public async Task<(bool success, string message)> DeleteAsync(int id)
        {
            var existing = await _categoryRepository.GetByIdAsync(id);
            if (existing == null)
            {
                return (false, "Không tìm thấy danh mục cần xóa.");
            }

            if (await _categoryRepository.HasProductsAsync(id))
            {
                return (false, $"Không thể xóa danh mục \"{existing.TenDanhMuc}\" vì đang có sản phẩm thuộc danh mục này. Bạn có thể chuyển sang trạng thái tạm ẩn.");
            }

            await _categoryRepository.DeleteAsync(existing);
            return (true, $"Đã xóa danh mục \"{existing.TenDanhMuc}\" thành công!");
        }

        public async Task<(bool success, string message)> ToggleStatusAsync(int id)
        {
            var existing = await _categoryRepository.GetByIdAsync(id);
            if (existing == null)
            {
                return (false, "Không tìm thấy danh mục.");
            }

            existing.DangHoatDong = !existing.DangHoatDong;
            await _categoryRepository.UpdateAsync(existing);

            var statusText = existing.DangHoatDong ? "kích hoạt" : "tạm ẩn";
            return (true, $"Đã {statusText} danh mục \"{existing.TenDanhMuc}\" thành công!");
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

            var folder = Path.Combine(webRootPath, "images", "danhmuc");
            Directory.CreateDirectory(folder);

            var fileName = $"dm_{Guid.NewGuid():N}{ext}";
            var fullPath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return (true, $"/images/danhmuc/{fileName}", string.Empty);
        }
    }
}
