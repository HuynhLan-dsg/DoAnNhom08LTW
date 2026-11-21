using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnNhom08LTW.Models
{
    public class DonHang
    {
        [Key]
        public int Id { get; set; }
        public string MaDonHang { get; set; }

        public int MaTaiKhoanId { get; set; }

        public DateTime NgayDat { get; set; }

        public string TenNguoiNhan { get; set; }

        public string SoDienThoai { get; set; }

        public string DiaChiGiao { get; set; }

        public decimal TongTien { get; set; }

        public string TrangThai { get; set; }

        public string? GhiChu { get; set; }

        public string PhuongThucThanhToan { get; set; }

        [ForeignKey("MaTaiKhoanId")]
        public TaiKhoan? TaiKhoan { get; set; }

        public ICollection<ChiTietDonHang>? ChiTietDonHangs { get; set; }
    }
}