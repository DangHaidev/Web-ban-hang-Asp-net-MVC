using Microsoft.AspNetCore.Mvc;

namespace Web_ban_hang.Areas.Admin.Controllers
{
    public class DataBoardController : Controller
    {
        [Area("Admin")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
