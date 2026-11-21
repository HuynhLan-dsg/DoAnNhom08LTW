using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace DoAnNhom08LTW.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")] // Chỉ tài khoản có VaiTro = 'Admin' mới được truy cập
    //Cơ chế hoạt động:Nếu chưa đăng nhập mà gõ /Admin/Dashboard ==> Tự động chuyển sang /Account/Login.
    //Nếu đăng nhập bằng tài khoản khach01 @phonestore.example(có VaiTro = 'KhachHang') mà cố tình gõ /Admin/Dashboard $\rightarrow$ Tự động chặn lại và chuyển sang /Account/AccessDenied.
    public class DashboardController: Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
