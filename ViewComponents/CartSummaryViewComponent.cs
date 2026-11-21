using Microsoft.AspNetCore.Mvc;

namespace DoAnNhom08LTW.ViewComponents
{
    public class CartSummaryViewComponent : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
