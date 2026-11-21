# Website Quản Lý Phụ Kiện Điện Thoại (DoAnNhom08LTW)

Dự án đồ án môn học Lập trình Web xây dựng hệ thống thương mại điện tử và quản lý bán phụ kiện điện thoại.

## 1. Công nghệ & Kiến trúc
- **Framework:** ASP.NET Core MVC (.NET 8)
- **ORM:** Entity Framework Core
- **Cơ sở dữ liệu:** SQL Server (`QuanLyPhuKienDienThoai`)
- **Kiến trúc:** MVC + Service + Repository Pattern

## 2. Phân công thành viên
| STT | Thành viên | Module phụ trách | Nhánh Git |
|-----|------------|------------------|-----------|
| 1 | Huỳnh Văn Lân | Tài khoản & Hệ thống | `feature/account-system` |
| 2 | Nguyễn Minh Thư | Sản phẩm & Mua hàng | `feature/product-shopping` |
| 3 | Phan Chí Thành | Admin & Đơn hàng | `feature/admin-dashboard` |

## 3. Hướng dẫn cài đặt & Chạy dự án
1. Mở **SQL Server Management Studio (SSMS)** và chạy file script `QuanLyPhuKienDienThoai.sql` để khởi tạo cơ sở dữ liệu và dữ liệu mẫu.
2. Mở file `appsettings.json` trong dự án, cập nhật chuỗi kết nối `"conn"` trỏ về SQL Server trên máy của bạn.
3. Mở Solution bằng **Visual Studio 2022** (hoặc chạy lệnh `dotnet run` trong terminal tại thư mục dự án).

## 4. Tài khoản Demo hệ thống
- **Tài khoản Admin:** `admin@phonestore.example`
- **Tài khoản Khách hàng:** `khach01@phonestore.example` đến `khach19@phonestore.example`
- **Mật khẩu chung:** `PhoneStore@2026`