using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DoAnNhom08LTW.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")] // Chỉ tài khoản có VaiTro = 'Admin' mới được truy cập
    public class PromotionController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
