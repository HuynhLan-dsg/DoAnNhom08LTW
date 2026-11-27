using DoAnNhom08LTW.Models.ViewModels;
using DoAnNhom08LTW.Services;
using DoAnNhom08LTW.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DoAnNhom08LTW.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly ILogger<AccountController> _logger;
        private readonly EmailService _emailservice;
        public AccountController(IAccountService accountService, ILogger<AccountController> logger, EmailService emailService)
        {
            _accountService = accountService;
            _emailservice = emailService;
            _logger = logger;
        }
        // 1. Hiển thị danh sách tài khoản (Khớp với link /Account/Index trong Sidebar Admin)
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var accounts = await _accountService.GetAllAccountsAsync();
            return View(accounts);
        }

        // 2. Xử lý Khóa / Mở khóa tài khoản
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int currentAdminId);

            var (success, text) = await _accountService.ToggleAccountStatusAsync(id, currentAdminId);
            if (success)
            {
                _logger.LogInformation("Admin ID {AdminId} đã thay đổi trạng thái tài khoản ID {TargetId}.", currentAdminId, id);
                TempData["SuccessMessage"] = text;
            }
            else
            {
                TempData["ErrorMessage"] = text;
            }

            return RedirectToAction("Index");
        }

        // 3. Xử lý Đổi quyền (Admin <-> KhachHang)
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(int id, string newRole)
        {
            int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out int currentAdminId);

            var (success, text) = await _accountService.ChangeAccountRoleAsync(id, newRole, currentAdminId);
            if (success)
            {
                _logger.LogInformation("Admin ID {AdminId} đã đổi quyền tài khoản ID {TargetId} thành {Role}.", currentAdminId, id, newRole);
                TempData["SuccessMessage"] = text;
            }
            else
            {
                TempData["ErrorMessage"] = text;
            }

            return RedirectToAction("Index");
        }
        // ================= 1. ĐĂNG NHẬP TRỰC TIẾP (KHÔNG CẦN OTP) =================
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _accountService.LoginAsync(model);
            if (user == null)
            {
                ModelState.AddModelError("", "Email hoặc mật khẩu không đúng, hoặc tài khoản đã bị khóa.");
                return View(model);
            }

            // Tạo Claims & đăng nhập thẳng bằng Cookie
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.HoTen),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.VaiTro)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

            _logger.LogInformation("Người dùng {Email} đã đăng nhập thành công.", user.Email);

            if (user.VaiTro == "Admin")
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
            }

            return RedirectToAction("Index", "Home");
        }

        // ================= 2. ĐĂNG KÝ + GỬI OTP XÁC THỰC EMAIL =================
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // 1. Kiểm tra email đã tồn tại chưa trước khi gửi OTP
            if (await _accountService.IsEmailExistsAsync(model.Email))
            {
                ModelState.AddModelError("", "Email này đã được sử dụng.");
                return View(model);
            }

            // 2. Tạo mã OTP 6 số & lưu tạm thông tin đăng ký vào Session
            string otp = new Random().Next(100000, 999999).ToString();

            HttpContext.Session.SetString("RegisterOTP", otp);
            HttpContext.Session.SetString("PendingRegisterData", System.Text.Json.JsonSerializer.Serialize(model));

            // 3. Gửi OTP qua Email
            await _emailservice.SendOtpEmail(model.Email, otp);
            _logger.LogInformation("Đã gửi mã OTP đăng ký tới {Email}: {Otp}", model.Email, otp);

            return RedirectToAction("VerifyRegisterOtp");
        }

        [HttpGet]
        public IActionResult VerifyRegisterOtp()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("PendingRegisterData")))
            {
                return RedirectToAction("Register");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyRegisterOtp(VerifyLoginOtpViewModel model)
        {
            string? sessionOtp = HttpContext.Session.GetString("RegisterOTP");
            string? pendingDataJson = HttpContext.Session.GetString("PendingRegisterData");

            if (string.IsNullOrEmpty(sessionOtp) || string.IsNullOrEmpty(pendingDataJson))
            {
                TempData["ErrorMessage"] = "Phiên xác thực đã hết hạn, vui lòng đăng ký lại.";
                return RedirectToAction("Register");
            }

            if (sessionOtp != model.OTP?.Trim())
            {
                ModelState.AddModelError("", "Mã OTP không chính xác.");
                return View(model);
            }

            // 4. OTP hợp lệ -> Lấy lại thông tin đăng ký và lưu chính thức vào SQL Server
            var registerModel = System.Text.Json.JsonSerializer.Deserialize<RegisterViewModel>(pendingDataJson);
            if (registerModel == null)
                return RedirectToAction("Register");

            var result = await _accountService.RegisterAsync(registerModel);
            if (!result)
            {
                ModelState.AddModelError("", "Email đã tồn tại trong hệ thống.");
                return View(model);
            }

            // 5. Xóa Session tạm sau khi hoàn tất
            HttpContext.Session.Remove("RegisterOTP");
            HttpContext.Session.Remove("PendingRegisterData");

            _logger.LogInformation("Tài khoản {Email} đã xác thực OTP và đăng ký thành công.", registerModel.Email);
            TempData["SuccessMessage"] = "Xác thực Email thành công! Vui lòng đăng nhập.";

            return RedirectToAction("Login");
        }
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            // Lấy Id (kiểu int) từ Cookie đăng nhập
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
            {
                return RedirectToAction("Login");
            }

            var profile = await _accountService.GetProfileAsync(userId);
            if (profile == null) return NotFound();

            return View(profile);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(ProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Profile", model);
            }

            var result = await _accountService.UpdateProfileAsync(model);
            if (result)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, model.Id.ToString()),
                    new Claim(ClaimTypes.Name, model.HoTen),
                    new Claim(ClaimTypes.Email, User.FindFirstValue(ClaimTypes.Email) ?? model.Email),
                    new Claim(ClaimTypes.Role, User.FindFirstValue(ClaimTypes.Role) ?? "KhachHang")
                };
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));
                _logger.LogInformation("Tài khoản ID {UserId} đã cập nhật hồ sơ.", model.Id);
                TempData["SuccessMessage"] = "Cập nhật thông tin cá nhân thành công!";
            }
            else
            {
                TempData["ErrorMessage"] = "Cập nhật thất bại, vui lòng thử lại.";
            }

            return RedirectToAction("Profile");
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(userIdString, out int userId))
            {
                return RedirectToAction("Login");
            }

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Dữ liệu mật khẩu chưa hợp lệ (mật khẩu mới tối thiểu 6 ký tự và phải trùng khớp).";
                return RedirectToAction("Profile");
            }

            var (success, message) = await _accountService.ChangePasswordAsync(userId, model);
            if (success)
            {
                _logger.LogInformation("Tài khoản ID {UserId} đã đổi mật khẩu.", userId);
                TempData["SuccessMessage"] = message;
            }
            else
            {
                TempData["ErrorMessage"] = message;
            }

            return RedirectToAction("Profile");
        }
        [Authorize]//Đảm bảo chỉ những người đang đăng nhập mới gọi được action này.
        public async Task<IActionResult> Logout()
        {
            // Lấy Email/ID để ghi log trước khi xóa Cookie
            var email = User.FindFirstValue(ClaimTypes.Email);

            // Xóa Cookie xác thực của trình duyệt
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            _logger.LogInformation("Người dùng {Email} đã đăng xuất.", email);

            // Chuyển hướng về trang chủ (hoặc trang Login)
            return RedirectToAction("Index", "Home");
        }

        //trang bao loi khong co quyen truy cap
        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
