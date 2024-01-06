using Microsoft.AspNetCore.Mvc;

namespace MultiShop.Areas.Admin.Controllers
{
    public class ErrorController:Controller
    {
        public IActionResult Index(string? mess, string code)
        {
            return View(new ErrorVM {Message = mess, Code  = code });
        }
    }
}
