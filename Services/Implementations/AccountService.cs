using DoAnNhom08LTW.Models;
using DoAnNhom08LTW.Models.ViewModels;
using DoAnNhom08LTW.Repositories.Interfaces;
using DoAnNhom08LTW.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace DoAnNhom08LTW.Services.Implementations
{
    public class AccountService: IAccountService
    {
        private readonly IAccountRepository _repository;
        private readonly PasswordHasher<TaiKhoan> _passwordHasher;

        public AccountService(
            IAccountRepository repository)
        {
            _repository = repository;
            _passwordHasher = new PasswordHasher<TaiKhoan>();
        }

        public async Task<bool> RegisterAsync(RegisterViewModel model)
        {
            var existingUser =
                await _repository.GetByEmailAsync(model.Email);

            if (existingUser != null)
                return false;

            var user = new TaiKhoan
            {
                HoTen = model.HoTen,
                Email = model.Email,
                VaiTro = "KhachHang",
                DangHoatDong = true,
                NgayTao = DateTime.Now
            };

            user.MatKhauHash =
                _passwordHasher.HashPassword(
                    user,
                    model.Password);

            await _repository.AddAsync(user);
            await _repository.SaveAsync();

            return true;
        }

        public async Task<TaiKhoan?> LoginAsync(LoginViewModel model)
        {
            var user = await _repository.GetByEmailAsync(model.Email);

            // Chặn luôn nếu không tìm thấy tài khoản HOẶC tài khoản đang bị khóa (DangHoatDong == false)
            if (user == null || !user.DangHoatDong)
                return null;

            var result = _passwordHasher.VerifyHashedPassword(
                user,
                user.MatKhauHash,
                model.Password);

            return result == PasswordVerificationResult.Success
                   ? user
                   : null;
        }

        // Thêm hàm này cho bước VerifyLoginOtp gọi
        public async Task<TaiKhoan?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        // 1. Lấy thông tin Profile đưa lên giao diện
        public async Task<ProfileViewModel?> GetProfileAsync(int id)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null) return null;

            return new ProfileViewModel
            {
                Id = user.Id,
                MaTaiKhoan = user.MaTaiKhoan ?? $"TK{user.Id:D4}",
                Email = user.Email,
                HoTen = user.HoTen,
                SoDienThoai = user.SoDienThoai,
                NgayTao = user.NgayTao
            };
        }

        // 2. Cập nhật họ tên & số điện thoại
        public async Task<bool> UpdateProfileAsync(ProfileViewModel model)
        {
            var user = await _repository.GetByIdAsync(model.Id);
            if (user == null) return false;

            user.HoTen = model.HoTen;
            user.SoDienThoai = model.SoDienThoai;

            await _repository.UpdateAsync(user);
            return true;
        }

        // 3. Đổi mật khẩu bằng PasswordHasher chuẩn của ASP.NET Core
        public async Task<(bool success, string text)> ChangePasswordAsync(int id, ChangePasswordViewModel model)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null) return (false, "Không tìm thấy tài khoản.");

            var verifyResult = _passwordHasher.VerifyHashedPassword(user, user.MatKhauHash, model.OldPassword);
            if (verifyResult == PasswordVerificationResult.Failed)
            {
                return (false, "Mật khẩu hiện tại không chính xác.");
            }

            user.MatKhauHash = _passwordHasher.HashPassword(user, model.NewPassword);
            await _repository.UpdateAsync(user);

            return (true, "Đổi mật khẩu thành công!");
        }

        // 1. Lấy danh sách tài khoản
        public async Task<IEnumerable<TaiKhoan>> GetAllAccountsAsync()
        {
            return await _repository.GetAllAsync();
        }

        // 2. Khóa / Mở khóa tài khoản (đảo trạng thái DangHoatDong)
        public async Task<(bool success, string text)> ToggleAccountStatusAsync(int id, int currentAdminId)
        {
            if (id == currentAdminId)
                return (false, "Bạn không thể tự khóa tài khoản của chính mình!");

            var user = await _repository.GetByIdAsync(id);
            if (user == null)
                return (false, "Không tìm thấy tài khoản.");

            user.DangHoatDong = !user.DangHoatDong;
            await _repository.UpdateAsync(user);

            string statusText = user.DangHoatDong ? "mở khóa" : "khóa";
            return (true, $"Đã {statusText} tài khoản {user.Email} thành công!");
        }

        // 3. Phân quyền (Admin / KhachHang)
        public async Task<(bool success, string text)> ChangeAccountRoleAsync(int id, string newRole, int currentAdminId)
        {
            if (id == currentAdminId)
                return (false, "Bạn không thể tự thay đổi quyền của chính mình!");

            if (newRole != "Admin" && newRole != "KhachHang")
                return (false, "Vai trò không hợp lệ.");

            var user = await _repository.GetByIdAsync(id);
            if (user == null)
                return (false, "Không tìm thấy tài khoản.");

            user.VaiTro = newRole;
            await _repository.UpdateAsync(user);

            return (true, $"Đã cập nhật quyền của {user.Email} thành {newRole}!");
        }
        public async Task<bool> IsEmailExistsAsync(string email)
        {
            var existingUser = await _repository.GetByEmailAsync(email);
            return existingUser != null;
        }
    }
}
