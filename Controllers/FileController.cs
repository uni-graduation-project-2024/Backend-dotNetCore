using Microsoft.AspNetCore.Mvc;

namespace Learntendo_backend.Controllers
{
    public class FileController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
