using System.Globalization;
using DoAnNhom08LTW.Models;

namespace DoAnNhom08LTW.Models.ViewModels.Admin
{
    public static class PromotionHelper
    {
        public const string PhanTram = "PhanTram";
        public const string SoTien = "SoTien";

        public static string NormalizeType(string? loaiGiam)
        {
            if (string.IsNullOrWhiteSpace(loaiGiam)) return PhanTram;
            var lower = loaiGiam.Trim().ToLowerInvariant();
            if (lower is "phantram" or "%" or "percent" or "phần trăm")
                return PhanTram;
            if (lower is "sotien" or "tienmat" or "vnd" or "vnđ" or "fixed" or "số tiền")
                return SoTien;
            return loaiGiam.Trim();
        }

        public static string GetDisplayType(string? loaiGiam)
        {
            return NormalizeType(loaiGiam) switch
            {
                PhanTram => "Giảm theo phần trăm (%)",
                SoTien => "Giảm theo số tiền (VNĐ)",
                _ => loaiGiam ?? string.Empty
            };
        }

        public static string FormatDiscount(string? loaiGiam, decimal giaTriGiam)
        {
            var viCulture = CultureInfo.GetCultureInfo("vi-VN");
            return NormalizeType(loaiGiam) == PhanTram
                ? $"{giaTriGiam:0.##}%"
                : $"{giaTriGiam.ToString("N0", viCulture)} ₫";
        }

        public static string GetStatusText(KhuyenMai km)
        {
            var now = DateTime.Now;
            if (!km.TrangThai) return "Tạm tắt";
            if (now < km.NgayBatDau) return "Sắp diễn ra";
            if (now > km.NgayKetThuc) return "Đã hết hạn";
            return "Đang áp dụng";
        }

        public static string GetStatusBadgeClass(KhuyenMai km)
        {
            var now = DateTime.Now;
            if (!km.TrangThai) return "bg-secondary-subtle text-secondary border border-secondary-subtle";
            if (now < km.NgayBatDau) return "bg-info-subtle text-info-emphasis border border-info-subtle";
            if (now > km.NgayKetThuc) return "bg-danger-subtle text-danger border border-danger-subtle";
            return "bg-success-subtle text-success border border-success-subtle";
        }

        public static bool IsCurrentlyActive(KhuyenMai km)
        {
            var now = DateTime.Now;
            return km.TrangThai && now >= km.NgayBatDau && now <= km.NgayKetThuc;
        }
    }
}
