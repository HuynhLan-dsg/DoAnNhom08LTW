# PHÂN CÔNG NHIỆM VỤ NHÓM

## Thông tin dự án

- **Tên đề tài:** Website Quản Lý Phụ Kiện Điện Thoại
- **Công nghệ:** ASP.NET Core MVC, Entity Framework Core, SQL Server
- **Kiến trúc:** MVC + Service + Repository Pattern
- **Cơ sở dữ liệu:** QuanLyPhuKienDienThoai

---

## Bảng phân công công việc

| STT | Thành viên | Module phụ trách | Công việc chính | Deadline |
|------|------------|------------------|-----------------|----------|
| 1 | Huỳnh Văn Lân | Tài khoản & Hệ thống | Đăng ký, Đăng nhập, Đăng xuất, Hồ sơ cá nhân, Phân quyền, Authentication, Authorization, Logging, Middleware | 17/11/2026 |
| 2 | Nguyễn Minh Thư | Sản phẩm & Mua hàng | Trang chủ, Sản phẩm, Tìm kiếm, Lọc sản phẩm, Giỏ hàng, Thanh toán, Responsive UI | 14/11/2026 |
| 3 | Phan Chí Thành | Admin & Đơn hàng | Quản lý sản phẩm, danh mục, khuyến mãi, đơn hàng, Dashboard thống kê, ViewComponent | 20/11/2026 |

---

# 1. Huỳnh Văn Lân

## Module: Tài khoản & Hệ thống

### Database

```text
TaiKhoan
```

### Model

```text
Models/
└── TaiKhoan.cs
```

### Repository

```text
Repositories/

Interfaces/
└── IAccountRepository.cs

Implementations/
└── AccountRepository.cs
```

### Service

```text
Services/

Interfaces/
└── IAccountService.cs

Implementations/
└── AccountService.cs
```

### Controller

```text
Controllers/
└── AccountController.cs
```

### View

```text
Views/
└── Account/
    ├── Index.cshtml
    ├── Login.cshtml
    ├── Register.cshtml
    └── Profile.cshtml
```

### Chức năng

- Đăng ký tài khoản
- Đăng nhập
- Đăng xuất
- Quản lý hồ sơ cá nhân
- Đổi mật khẩu
- Phân quyền Admin/User
- Khóa/Mở khóa tài khoản

### Hệ thống

- Kết nối SQL Server
- Cấu hình ApplicationDbContext
- Migration Database
- Cookie Authentication
- Role Authorization
- Logging bằng ILogger
- Middleware xử lý lỗi

### Tài liệu

```text
README.md
DECISIONS.md
```

### Branch Git

```bash
feature/account-system
```

---

# 2. Nguyễn Minh Thư

## Module: Sản phẩm & Mua hàng

### Database

```text
DanhMuc
KhuyenMai
SanPham
```

### Model

```text
Models/
├── DanhMuc.cs
├── KhuyenMai.cs
└── SanPham.cs
```

### Repository

```text
Repositories/

Interfaces/
└── IProductRepository.cs

Implementations/
└── ProductRepository.cs
```

### Service

```text
Services/

Interfaces/
└── IProductService.cs

Implementations/
└── ProductService.cs
```

### Controller

```text
Controllers/
├── HomeController.cs
├── ProductController.cs
└── CartController.cs
```

### View

```text
Views/
├── Home/
├── Product/
└── Cart/
```

### Shared UI

```text
Views/Shared/

├── _Layout_user.cshtml
├── _Header.cshtml
├── _Footer.cshtml
├── _SearchBar.cshtml
└── _Pagination.cshtml
```

### Chức năng

#### Trang chủ

- Hiển thị sản phẩm nổi bật
- Hiển thị sản phẩm mới
- Hiển thị chương trình khuyến mãi

#### Sản phẩm

- Danh sách sản phẩm
- Chi tiết sản phẩm
- Tìm kiếm sản phẩm
- Lọc theo danh mục
- Lọc theo thương hiệu

#### Giỏ hàng

- Thêm sản phẩm vào giỏ hàng
- Cập nhật số lượng
- Xóa sản phẩm khỏi giỏ hàng
- Thanh toán
- Tạo đơn hàng

### Giao diện

- Responsive Desktop
- Responsive Tablet
- Responsive Mobile

### Tài liệu

```text
TESTPLAN.md
```

### Branch Git

```bash
feature/product-shopping
```

---

# 3. Phan Chí Thành

## Module: Admin & Đơn hàng

### Database

```text
DonHang
ChiTietDonHang
```

### Model

```text
Models/
├── DonHang.cs
└── ChiTietDonHang.cs
```

### Repository

```text
Repositories/

Interfaces/
├── ICategoryRepository.cs
└── IOrderRepository.cs

Implementations/
├── CategoryRepository.cs
└── OrderRepository.cs
```

### Service

```text
Services/

Interfaces/
├── ICategoryService.cs
└── IOrderService.cs

Implementations/
├── CategoryService.cs
└── OrderService.cs
```

### Controller

```text
Areas/Admin/Controllers/

├── DashboardController.cs
├── CategoryController.cs
├── ProductController.cs
├── PromotionController.cs
└── OrderController.cs
```

### View

```text
Areas/Admin/Views/

├── Dashboard/
├── Category/
├── Product/
├── Promotion/
└── Order/
```

### Chức năng

#### Quản lý danh mục

- Thêm danh mục
- Sửa danh mục
- Xóa danh mục
- Xem danh sách danh mục

#### Quản lý sản phẩm

- Thêm sản phẩm
- Sửa sản phẩm
- Xóa mềm sản phẩm
- Quản lý tồn kho

#### Quản lý khuyến mãi

- Thêm khuyến mãi
- Sửa khuyến mãi
- Xóa khuyến mãi

#### Quản lý đơn hàng

- Xem danh sách đơn hàng
- Xem chi tiết đơn hàng
- Xác nhận đơn hàng
- Hủy đơn hàng
- Cập nhật trạng thái giao hàng

#### Dashboard

- Tổng số sản phẩm
- Tổng số đơn hàng
- Tổng doanh thu
- Thống kê doanh thu theo tháng

### ViewComponent

```text
ViewComponents/

├── CartSummaryViewComponent.cs
└── NotificationViewComponent.cs
```

### Tài liệu

```text
ARCHITECTURE.md
```

### Branch Git

```bash
feature/admin-dashboard
```

---

# Git Branch

| Thành viên | Branch |
|------------|---------|
| Huỳnh Văn Lân | feature/account-system |
| Nguyễn Minh Thư | feature/product-shopping |
| Phan Chí Thành | feature/admin-dashboard |

---

# Tài liệu dự án

| Tài liệu | Người phụ trách |
|-----------|----------------|
| README.md | Huỳnh Văn Lân |
| DECISIONS.md | Huỳnh Văn Lân |
| TESTPLAN.md | Nguyễn Minh Thư |
| ARCHITECTURE.md | Phan Chí Thành |
| TASKS.md | Cả nhóm |
| MEETING_NOTES.md | Cả nhóm |
| PEER_REVIEW.md | Cả nhóm |

---

# Biên bản họp

Nhóm thực hiện tối thiểu 4 Sprint:

| Sprint | Nội dung |
|----------|----------|
| Sprint 1 | Phân tích yêu cầu và thiết kế cơ sở dữ liệu |
| Sprint 2 | Xây dựng các chức năng chính |
| Sprint 3 | Kiểm thử và sửa lỗi |
| Sprint 4 | Hoàn thiện tài liệu, tối ưu giao diện và chuẩn bị demo |

Biên bản họp được lưu trong:

```text
MEETING_NOTES.md
```

---

# Quy trình Git

1. Tạo branch cá nhân từ `main`.
2. Thực hiện các chức năng được phân công.
3. Commit theo từng chức năng nhỏ.
4. Push code lên GitHub.
5. Tạo Pull Request.
6. Thành viên khác review.
7. Merge vào `main` sau khi được duyệt.

### Ví dụ

```bash
git checkout -b feature/account-system

git add .

git commit -m "feat: implement login and register"

git push origin feature/account-system
```

---

# Quy tắc làm việc

- Không commit trực tiếp lên `main`.
- Mỗi thành viên làm việc trên branch riêng.
- Mỗi chức năng phải có commit rõ ràng.
- Tạo Pull Request trước khi merge.
- Thực hiện review chéo giữa các thành viên.
- Cập nhật tiến độ thường xuyên thông qua GitHub.

---

# Ước lượng khối lượng công việc

| Thành viên | Tỷ lệ |
|------------|--------|
| Huỳnh Văn Lân | ~33% |
| Nguyễn Minh Thư | ~34% |
| Phan Chí Thành | ~33% |

**Mục tiêu:** Mỗi thành viên phụ trách trọn vẹn một module gồm Model → Repository → Service → Controller → View nhằm đảm bảo đóng góp đồng đều, hạn chế xung đột mã nguồn và thuận lợi trong quá trình bảo vệ đồ án.