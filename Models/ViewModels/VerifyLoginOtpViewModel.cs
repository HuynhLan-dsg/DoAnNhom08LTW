using System.ComponentModel.DataAnnotations;

namespace DoAnNhom08LTW.Models.ViewModels
{
    public class VerifyLoginOtpViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập mã OTP.")]
        [StringLength(6, MinimumLength = 6, ErrorMessage = "Mã OTP phải gồm đúng 6 chữ số.")]
        public string OTP { get; set; } = string.Empty;
    }
}