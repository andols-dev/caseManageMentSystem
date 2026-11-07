using Microsoft.AspNetCore.Mvc;

namespace caseManageMentSystem.Areas.CaseManager.Controllers;
[Area("CaseManager")]
public class ChatController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}