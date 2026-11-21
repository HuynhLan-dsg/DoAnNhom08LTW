using Microsoft.AspNetCore.Mvc;

namespace DoAnNhom08LTW.ViewComponents
{
    public class NotificationViewComponent : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
