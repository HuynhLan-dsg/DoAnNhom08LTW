# Quyết Định Kỹ Thuật (Technical Decisions)

Tài liệu ghi nhận các lựa chọn kiến trúc và giải pháp kỹ thuật áp dụng trong hệ thống.

## 1. Kiến trúc phân tầng (MVC + Service + Repository)
- **Quyết định:** Tách mã nguồn thành 3 lớp độc lập: `Controller` (xử lý HTTP request/response), `Service` (xử lý nghiệp vụ, mapping ViewModel, mã hóa mật khẩu), và `Repository` (truy vấn Entity Framework Core).
- **Lý do:** Giúp tái sử dụng các hàm truy vấn cơ bản, tách biệt logic nghiệp vụ khỏi giao diện, và hạn chế xung đột mã nguồn khi 3 thành viên làm việc song song.

## 2. Xác thực & Phân quyền bằng Cookie Authentication
- **Quyết định:** Sử dụng `CookieAuthenticationDefaults` của ASP.NET Core thay vì tự lưu Session thủ công. Lưu các thông tin thiết yếu (`Id`, `HoTen`, `Email`, `VaiTro`) vào `Claims`.
- **Lý do:** Hỗ trợ cơ chế phân quyền chuẩn xác thông qua Attribute `[Authorize(Roles = "Admin")]`, tự động chuyển hướng sang `/Account/Login` hoặc `/Account/AccessDenied` khi truy cập trái phép.

## 3. Bảo mật mật khẩu bằng `PasswordHasher`
- **Quyết định:** Sử dụng lớp `PasswordHasher<TaiKhoan>` của `Microsoft.AspNetCore.Identity` để băm (hash) và kiểm tra mật khẩu.
- **Lý do:** Đảm bảo an toàn thông tin người dùng, không lưu mật khẩu dạng văn bản thuần (plain text) trong cơ sở dữ liệu.

## 4. Cấu hình Computed Column trong EF Core
- **Quyết định:** Khai báo `HasComputedColumnSql` trong `ApplicationDbContext.OnModelCreating` cho cột `MaTaiKhoan` (`PERSISTED`).
- **Lý do:** Đồng bộ giữa mô hình đối tượng C# và cột tự động sinh mã của SQL Server, tránh lỗi `SqlException` khi thực hiện các thao tác `INSERT` hoặc `UPDATE`.

## 5. Xử lý ngoại lệ tập trung với Custom Middleware
- **Quyết định:** Xây dựng `ExceptionMiddleware` đặt ở đầu HTTP Request Pipeline kết hợp với `ILogger`.
- **Lý do:** Bắt toàn bộ lỗi hệ thống chưa được xử lý, ghi log chi tiết để phục vụ debug và điều hướng người dùng về trang thông báo lỗi thân thiện (`/Home/Error`).