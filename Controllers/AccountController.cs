using DoAnNhom08LTW.Models.ViewModels;
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
        public AccountController(IAccountService accountService, ILogger<AccountController> logger)
        {
            _accountService = accountService;
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
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login( LoginViewModel model)
        {
            //Kiem tra validation
            if(!ModelState.IsValid)
                return View(model);
            //Kiem tra tai khoan ton tai khong
            var user = await _accountService.LoginAsync(model);

            //Neu khong co tai khoan
            if(user  == null)
            {
                ModelState.AddModelError("", "Email hoac mat khau khogn dung");
                return View(model);// tra ve form
            }

            //tao claimns: claim thong tin nguoi dung sau khi nhap 
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), //luu id
                new Claim(ClaimTypes.Name, user.HoTen), // luu ho ten
                new Claim(ClaimTypes.Email, user.Email), //luu email
                new Claim(ClaimTypes.Role, user.VaiTro), //luu vai tro; dung de phan quyen
            };
            //tao Identity 


            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme); // tao danh tunh chua (id, hoten, email, role)

            //Tao principal
            var principal  = new ClaimsPrincipal(identity);// nguoi dung

            //dang nhap
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            _logger.LogInformation("Người dùng {Email} (Quyền: {Role}) đã đăng nhập.", user.Email, user.VaiTro);

            // Điều hướng theo vai trò
            if (user.VaiTro == "Admin")
            {
                return RedirectToAction("Index", "Dashboard", new { area = "Admin" });
                // (Nếu nhóm chưa tạo Area Admin thì tạm thời trỏ về trang quản lý tài khoản: RedirectToAction("Index", "Account"))
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(
            RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result =
                await _accountService.RegisterAsync(model);

            if (!result)
            {
                ModelState.AddModelError(
                    "",
                    "Email đã tồn tại");

                return View(model);
            }

            TempData["Success"] =
                "Đăng ký thành công";

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
