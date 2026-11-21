using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAnNhom08LTW.Models
{
    public class ChiTietDonHang
    {
        [Key]
        public int Id { get; set; }
        public string MaChiTiet {  get; set; }
        public int MaDonHangId { get; set; }

        public int MaSanPhamId { get; set; }

        public int SoLuong { get; set; }

        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }

        [ForeignKey("MaDonHangId")]
        public DonHang? DonHang { get; set; }

        [ForeignKey("MaSanPhamId")]
        public SanPham? SanPham { get; set; }
    }
}