using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAnNhom08LTW.Controllers
{
    [Authorize]//// Chỉ cần đăng nhập, không phân biệt Admin hay KhachHang
    public class OrderController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
