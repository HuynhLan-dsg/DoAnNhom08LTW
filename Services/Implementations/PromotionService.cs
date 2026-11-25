using DoAnNhom08LTW.Models;
using DoAnNhom08LTW.Models.ViewModels.Admin;
using DoAnNhom08LTW.Repositories.Interfaces;
using DoAnNhom08LTW.Services.Interfaces;

namespace DoAnNhom08LTW.Services.Implementations
{
    public class PromotionService : IPromotionService
    {
        private readonly IPromotionRepository _promotionRepository;

        public PromotionService(IPromotionRepository promotionRepository)
        {
            _promotionRepository = promotionRepository;
        }

        public async Task<IEnumerable<KhuyenMai>> GetAllAsync(string? keyword = null, bool? trangThai = null)
        {
            return await _promotionRepository.GetAllAsync(keyword, trangThai);
        }

        public async Task<IEnumerable<KhuyenMai>> GetActiveAsync()
        {
            return await _promotionRepository.GetActiveAsync();
        }

        public async Task<KhuyenMai?> GetByIdAsync(int id)
        {
            return await _promotionRepository.GetByIdAsync(id);
        }

        public async Task<PromotionFormViewModel?> GetFormByIdAsync(int id)
        {
            var promo = await _promotionRepository.GetByIdAsync(id);
            if (promo == null) return null;

            return new PromotionFormViewModel
            {
                Id = promo.Id,
                MaKhuyenMai = promo.MaKhuyenMai,
                TenKhuyenMai = promo.TenKhuyenMai,
                LoaiGiam = PromotionHelper.NormalizeType(promo.LoaiGiam),
                GiaTriGiam = promo.GiaTriGiam,
                NgayBatDau = promo.NgayBatDau,
                NgayKetThuc = promo.NgayKetThuc,
                TrangThai = promo.TrangThai
            };
        }

        public async Task<(bool success, string message)> CreateAsync(PromotionFormViewModel model)
        {
            var validationError = ValidatePromotion(model);
            if (validationError != null)
            {
                return (false, validationError);
            }

            var entity = new KhuyenMai
            {
                TenKhuyenMai = model.TenKhuyenMai.Trim(),
                LoaiGiam = PromotionHelper.NormalizeType(model.LoaiGiam),
                GiaTriGiam = model.GiaTriGiam,
                NgayBatDau = model.NgayBatDau,
                NgayKetThuc = model.NgayKetThuc,
                TrangThai = model.TrangThai
            };

            await _promotionRepository.AddAsync(entity);
            await _promotionRepository.SaveAsync();

            return (true, $"Thêm khuyến mãi \"{entity.TenKhuyenMai}\" thành công!");
        }

        public async Task<(bool success, string message)> UpdateAsync(PromotionFormViewModel model)
        {
            var existing = await _promotionRepository.GetByIdAsync(model.Id);
            if (existing == null)
            {
                return (false, "Không tìm thấy chương trình khuyến mãi.");
            }

            var validationError = ValidatePromotion(model);
            if (validationError != null)
            {
                return (false, validationError);
            }

            existing.TenKhuyenMai = model.TenKhuyenMai.Trim();
            existing.LoaiGiam = PromotionHelper.NormalizeType(model.LoaiGiam);
            existing.GiaTriGiam = model.GiaTriGiam;
            existing.NgayBatDau = model.NgayBatDau;
            existing.NgayKetThuc = model.NgayKetThuc;
            existing.TrangThai = model.TrangThai;

            await _promotionRepository.UpdateAsync(existing);
            return (true, $"Cập nhật khuyến mãi \"{existing.TenKhuyenMai}\" thành công!");
        }

        public async Task<(bool success, string message)> DeleteAsync(int id)
        {
            var existing = await _promotionRepository.GetByIdAsync(id);
            if (existing == null)
            {
                return (false, "Không tìm thấy chương trình khuyến mãi cần xóa.");
            }

            await _promotionRepository.DetachProductsAsync(id);
            await _promotionRepository.DeleteAsync(existing);

            return (true, $"Đã xóa chương trình khuyến mãi \"{existing.TenKhuyenMai}\" thành công!");
        }

        public async Task<(bool success, string message)> ToggleStatusAsync(int id)
        {
            var existing = await _promotionRepository.GetByIdAsync(id);
            if (existing == null)
            {
                return (false, "Không tìm thấy chương trình khuyến mãi.");
            }

            existing.TrangThai = !existing.TrangThai;
            await _promotionRepository.UpdateAsync(existing);

            var statusText = existing.TrangThai ? "kích hoạt" : "tạm tắt";
            return (true, $"Đã {statusText} khuyến mãi \"{existing.TenKhuyenMai}\" thành công!");
        }

        private static string? ValidatePromotion(PromotionFormViewModel model)
        {
            if (model.NgayKetThuc < model.NgayBatDau)
            {
                return "Ngày kết thúc phải lớn hơn hoặc bằng ngày bắt đầu.";
            }

            if (model.GiaTriGiam <= 0)
            {
                return "Giá trị giảm phải lớn hơn 0.";
            }

            var normalizedType = PromotionHelper.NormalizeType(model.LoaiGiam);
            if (normalizedType != PromotionHelper.PhanTram && normalizedType != PromotionHelper.SoTien)
            {
                return "Loại giảm giá không hợp lệ.";
            }

            if (normalizedType == PromotionHelper.PhanTram && model.GiaTriGiam > 100)
            {
                return "Giá trị giảm theo phần trăm phải nằm trong khoảng 0% đến 100%.";
            }

            return null;
        }
    }
}
