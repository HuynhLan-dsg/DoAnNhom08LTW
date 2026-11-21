using System.ComponentModel.DataAnnotations;

namespace DoAnNhom08LTW.Models
{
    public class KhuyenMai
    {
        [Key]
        public int Id { get; set; }
        public string MaKhuyenMai { get; set; }

        public string TenKhuyenMai { get; set; }

        public string LoaiGiam { get; set; }

        public decimal GiaTriGiam { get; set; }

        public DateTime NgayBatDau { get; set; }

        public DateTime NgayKetThuc { get; set; }

        public bool TrangThai { get; set; }

        public ICollection<SanPham>? SanPhams { get; set; }
    }
}
