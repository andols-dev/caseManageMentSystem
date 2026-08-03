using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace caseManageMentSystem.Areas.Client.Controllers
{
    [Area("Client")]
    [Authorize(Roles ="client")]
    public class DashBoardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
