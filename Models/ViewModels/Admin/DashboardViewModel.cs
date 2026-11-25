using DoAnNhom08LTW.Models;

namespace DoAnNhom08LTW.Models.ViewModels.Admin
{
    public class MonthlyRevenueItem
    {
        public int Thang { get; set; }
        public decimal DoanhThu { get; set; }
        public int SoDonHang { get; set; }
    }

    public class DashboardViewModel
    {
        public decimal TongDoanhThu { get; set; }
        public int TongDonHang { get; set; }
        public int DonHangChoXacNhan { get; set; }
        public int TongSanPham { get; set; }
        public int SanPhamDangBan { get; set; }
        public int SanPhamSapHetHang { get; set; }
        public int TongKhachHang { get; set; }
        public int NamThongKe { get; set; }
        public List<int> DanhSachNam { get; set; } = new();
        public List<MonthlyRevenueItem> DoanhThuTheoThang { get; set; } = new();
        public List<DonHang> DonHangGanDay { get; set; } = new();
    }

    public class CartSummaryViewModel
    {
        public int TotalQuantity { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class NotificationItemViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string BadgeText { get; set; } = string.Empty;
        public string BadgeClass { get; set; } = "bg-warning text-dark";
        public string IconClass { get; set; } = "bi-bell";
        public string Area { get; set; } = "Admin";
        public string Controller { get; set; } = "Dashboard";
        public string Action { get; set; } = "Index";
        public Dictionary<string, string>? RouteValues { get; set; }
    }

    public class NotificationViewModel
    {
        public int PendingOrderCount { get; set; }
        public int LowStockProductCount { get; set; }
        public int ExpiringPromotionCount { get; set; }
        public int TotalAlertCount => PendingOrderCount + LowStockProductCount + ExpiringPromotionCount;
        public List<NotificationItemViewModel> Items { get; set; } = new();
    }
}
