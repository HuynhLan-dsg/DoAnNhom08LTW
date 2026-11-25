using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace DoAnNhom08LTW.Models.ViewModels.Admin
{
    public class CategoryFormViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Mã danh mục")]
        public string? MaDanhMuc { get; set; }

        [Required(ErrorMessage = "Nhập tên danh mục")]
        [StringLength(100, ErrorMessage = "Tên danh mục tối đa 100 ký tự")]
        [Display(Name = "Tên danh mục")]
        public string TenDanhMuc { get; set; } = string.Empty;

        [StringLength(255, ErrorMessage = "Đường dẫn hình ảnh tối đa 255 ký tự")]
        [Display(Name = "Đường dẫn hình ảnh")]
        public string? HinhAnh { get; set; }

        [Display(Name = "Tải ảnh lên")]
        public IFormFile? HinhAnhFile { get; set; }

        [Display(Name = "Đang hoạt động")]
        public bool DangHoatDong { get; set; } = true;
    }

    public class ProductFormViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Mã sản phẩm")]
        public string? MaSanPham { get; set; }

        [Required(ErrorMessage = "Nhập tên sản phẩm")]
        [StringLength(150, ErrorMessage = "Tên sản phẩm tối đa 150 ký tự")]
        [Display(Name = "Tên sản phẩm")]
        public string TenSanPham { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vui lòng chọn danh mục")]
        [Range(1, int.MaxValue, ErrorMessage = "Vui lòng chọn danh mục hợp lệ")]
        [Display(Name = "Danh mục")]
        public int MaDanhMucId { get; set; }

        [Display(Name = "Chương trình khuyến mãi")]
        public int? MaKhuyenMaiId { get; set; }

        [Required(ErrorMessage = "Nhập thương hiệu")]
        [StringLength(100, ErrorMessage = "Thương hiệu tối đa 100 ký tự")]
        [Display(Name = "Thương hiệu")]
        public string ThuongHieu { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nhập giá bán")]
        [Range(0, 999999999, ErrorMessage = "Giá bán phải lớn hơn hoặc bằng 0")]
        [Display(Name = "Giá bán (VNĐ)")]
        public decimal GiaBan { get; set; }

        [Required(ErrorMessage = "Nhập số lượng tồn")]
        [Range(0, 1000000, ErrorMessage = "Số lượng tồn phải lớn hơn hoặc bằng 0")]
        [Display(Name = "Số lượng tồn kho")]
        public int SoLuongTon { get; set; }

        [StringLength(255, ErrorMessage = "Đường dẫn hình ảnh tối đa 255 ký tự")]
        [Display(Name = "Đường dẫn hình ảnh")]
        public string? HinhAnh { get; set; }

        [Display(Name = "Tải ảnh sản phẩm")]
        public IFormFile? HinhAnhFile { get; set; }

        [Display(Name = "Mô tả sản phẩm")]
        public string? MoTa { get; set; }

        [Display(Name = "Đang bán")]
        public bool DangBan { get; set; } = true;

        public DateTime NgayTao { get; set; } = DateTime.Now;
    }

    public class PromotionFormViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Mã khuyến mãi")]
        public string? MaKhuyenMai { get; set; }

        [Required(ErrorMessage = "Nhập tên chương trình khuyến mãi")]
        [StringLength(150, ErrorMessage = "Tên khuyến mãi tối đa 150 ký tự")]
        [Display(Name = "Tên khuyến mãi")]
        public string TenKhuyenMai { get; set; } = string.Empty;

        [Required(ErrorMessage = "Chọn loại giảm giá")]
        [Display(Name = "Loại giảm")]
        public string LoaiGiam { get; set; } = PromotionHelper.PhanTram;

        [Required(ErrorMessage = "Nhập giá trị giảm")]
        [Range(0.01, 999999999, ErrorMessage = "Giá trị giảm phải lớn hơn 0")]
        [Display(Name = "Giá trị giảm")]
        public decimal GiaTriGiam { get; set; }

        [Required(ErrorMessage = "Chọn ngày bắt đầu")]
        [Display(Name = "Ngày bắt đầu")]
        [DataType(DataType.DateTime)]
        public DateTime NgayBatDau { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Chọn ngày kết thúc")]
        [Display(Name = "Ngày kết thúc")]
        [DataType(DataType.DateTime)]
        public DateTime NgayKetThuc { get; set; } = DateTime.Now.AddDays(30);

        [Display(Name = "Kích hoạt")]
        public bool TrangThai { get; set; } = true;
    }
}
