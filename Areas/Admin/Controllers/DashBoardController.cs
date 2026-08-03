using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace caseManageMentSystem.Areas.Admin.Controllers
{
    [Area("admin")]
    [Authorize(Roles ="admin")]
    public class DashBoardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
