using Microsoft.AspNetCore.Mvc;

namespace DoAnNhom08LTW.Controllers
{
    public class ProductController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
