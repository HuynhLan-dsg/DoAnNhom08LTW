using System.ComponentModel.DataAnnotations;

namespace DoAnNhom08LTW.Models
{
    public class TaiKhoan
    {
        [Key]
        public int Id { get; set; }
        public string MaTaiKhoan {  get; set; }

        public string HoTen { get; set; }

        public string Email { get; set; }
            
        public string MatKhauHash { get; set; }

        public string? SoDienThoai { get; set; }

        public string VaiTro { get; set; }

        public bool DangHoatDong { get; set; }

        public DateTime NgayTao { get; set; }
    }
}
