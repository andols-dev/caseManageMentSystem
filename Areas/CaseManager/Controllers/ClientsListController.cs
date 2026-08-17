using caseManageMentSystem.Areas.CaseManager.ViewModels;
using caseManageMentSystem.Data;
using caseManageMentSystem.Enums;
using caseManageMentSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace caseManageMentSystem.Areas.CaseManager.Controllers
{
    [Area("CaseManager")]
    [Authorize(Roles = "caseManager")]
    public class ClientsListController(ApplicationDbContext context, UserManager<ApplicationUser> userManager) : Controller
    {
        // GET: ClientsListController
        // Show all clients

        private readonly ApplicationDbContext _context = context;
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        public async Task<IActionResult> Index()
        {


            // get all the clients(users) where the role is client

            var clients = await _userManager.GetUsersInRoleAsync("client");


            return View(clients);
        }

        // GET: ClientsListController/Details/5
        public async Task<IActionResult> Details(string id)
        {

            // var client = _context.Users.Find(id);

            // todo: also add caseManager full name


            //var clientAndCases = await _context.Users
            //    .Where(u => u.Id == id)
            //    .Select(u => new ClientViewModel
            //    {
            //        Name = u.FullName,
            //        Cases = u.ClientCases.Select(c => new ClientCaseViewModel
            //        {

            //        })
            //    })





            var clientAndCases = await _context.Users
                .Include(c => c.ClientCases)
                .ThenInclude(c => c.CaseManager)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (clientAndCases == null) {
                return NotFound();
            }

            //todo: load in cases if the user has any
            return View(clientAndCases);
        }

        // GET: ClientsListController/Create
        public ActionResult Create(string clientId)
        {
            ViewBag.ClientId = clientId;
            // create viewmodel
            return View();
        }

        // POST: ClientsListController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateCaseViewModel caseItem)
        {
            
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
            {
                return Unauthorized();
            }

            if (ModelState.IsValid)
            {
                var newCase = new Case
                {
                    ClientId = caseItem.ClientId,
                    Title = caseItem.Title,
                    Description = caseItem.Description,
                    Status = Enums.Status.active,
                    CaseNumber = GenerateCaseNumber(),
                    CreatedDate = DateTime.Now,
                    CaseManagerId = currentUser.Id,
                };

                _context.Cases.Add(newCase);
                await _context.SaveChangesAsync();

                var newCaseHistory = new CaseHistory
                {
                    CaseId = newCase.Id,
                    UserId = currentUser.Id,
                    Type = CaseHistoryType.CaseCreated,
                    CreatedDate = DateTime.Now,
                };

                _context.CaseHistories.Add(newCaseHistory);
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "DashBoard");
            }
            return View(caseItem);
        }

        // GET: ClientsListController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: ClientsListController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: ClientsListController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: ClientsListController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // create a case number function

        private static string GenerateCaseNumber()
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

            var result = new StringBuilder(8);

            for (int i = 0; i < 8; i++)
            {
                result.Append(chars[RandomNumberGenerator.GetInt32(chars.Length)]);
            }

            return $"AR-{result}";
        }
    }
}
