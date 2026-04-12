using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
// ...existing code...

namespace caseManageMentSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        public HomeController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }
        // GET: HomeController
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterUser regUser)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser
                {
                    UserName = regUser.Email,
                    Email = regUser.Email,
                };
                var result = await _userManager.CreateAsync(user, regUser.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "client");
                    return RedirectToAction("Index");
                }
                else
                {
                    // Handle registration errors
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(regUser);
        }
    }
}
