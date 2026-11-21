using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnNhom08LTW.Models
{
    public class SanPham
    {
        [Key]
        public int Id { get; set; }
        public string MaSanPham {  get; set; }

        public string TenSanPham { get; set; }

        public int MaDanhMucId { get; set; }

        public int? MaKhuyenMaiId { get; set; }

        public string ThuongHieu { get; set; }

        public decimal GiaBan { get; set; }

        public int SoLuongTon { get; set; }

        public string? HinhAnh { get; set; }

        public string? MoTa { get; set; }

        public bool DangBan { get; set; }

        public DateTime NgayTao { get; set; }

        [ForeignKey("MaDanhMucId")]
        public DanhMuc? DanhMuc { get; set; }

        [ForeignKey("MaKhuyenMaiId")]
        public KhuyenMai? KhuyenMai { get; set; }

        public ICollection<ChiTietDonHang>? ChiTietDonHangs { get; set; }
    }
}