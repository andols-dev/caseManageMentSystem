using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using caseManageMentSystem.Models;
using caseManageMentSystem.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace caseManageMentSystem.Controllers
{
    public class AccountController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        // GET: AccountController
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
                var user = new ApplicationUser
                {
                    FirstName = regUser.FirstName,
                    LastName = regUser.LastName,
                    UserName = regUser.Email,
                    Email = regUser.Email,
                };
                var result = await _userManager.CreateAsync(user, regUser.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, "client");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
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
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginUser loginUser)
        {
            var modelStateErrors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(loginUser.Email, loginUser.Password, isPersistent: loginUser.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt. Please check your email and password.");
                    ViewBag.Error = true;
                }


            }
            return View(loginUser);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(LoggedOut));
        }

        public IActionResult LoggedOut()
        {
            return View();
        }

        // User profile page

        public async Task<IActionResult> UserProfile()
        {
            var userId = _userManager.GetUserId(User);

            var userProfileInfo = await _userManager.Users
                .Where(u => u.Id == userId)
                .Select(u => new UserProfileViewModel
                {
                    Email = u.Email,
                    UserName = u.UserName,
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    FullName = u.FullName,
                    PhoneNumber = u.PhoneNumber,
                    Id = userId,

                })
                .FirstOrDefaultAsync();

            if (userProfileInfo == null)
            {
                return NotFound();
            }

            return View(userProfileInfo);
        }

        [HttpGet]
        public IActionResult EditUserProfile(string id)
        {
            var user = _userManager.Users.FirstOrDefault(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            var userInfo = new UserProfileViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                FullName = user.FullName
            };


            return View(userInfo);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditUserProfile(UserProfileViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id == null)
                {
                    return NotFound();
                }

                var user = await _userManager.FindByIdAsync(model.Id);
                if (user == null)
                {
                    return NotFound();
                }

                user.FirstName = model.FirstName ?? string.Empty;
                user.LastName = model.LastName ?? string.Empty;
                user.Email = model.Email ?? string.Empty;
                user.PhoneNumber = model.PhoneNumber;

                var result = await _userManager.UpdateAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(UserProfile));
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View(new ChangePasswordViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return NotFound();
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();
            }

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                return RedirectToAction(nameof(UserProfile));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }
    }
}