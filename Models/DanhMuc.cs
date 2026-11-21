using System.ComponentModel.DataAnnotations;

namespace DoAnNhom08LTW.Models
{
    public class DanhMuc
    {
        [Key]
        public int Id { get; set; }
        public string MaDanhMuc { get; set; }

        public string TenDanhMuc { get; set; }

        public string? HinhAnh { get; set; }

        public bool DangHoatDong { get; set; }

        public ICollection<SanPham>? SanPhams { get; set; }
    }
}