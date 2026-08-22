using caseManageMentSystem.Areas.CaseManager.ViewModels;
using caseManageMentSystem.Data;
using caseManageMentSystem.Enums;
using caseManageMentSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using caseManageMentSystem.Services;



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
            var clients = await _userManager.GetUsersInRoleAsync("client");
            return View(clients);
        }

        // GET: ClientsListController/Details/5
        public async Task<IActionResult> Details(string id)
        {

            var clientAndCases = await _context.Users
                .Include(c => c.ClientCases)
                .ThenInclude(c => c.CaseManager)
                .FirstOrDefaultAsync(u => u.Id == id);

            if (clientAndCases == null) {
                return NotFound();
            }


            return View(clientAndCases);
        }

        // GET: ClientsListController/CaseDetails/5
        public async Task<IActionResult> CaseDetails(int caseId)
        {

            var caseItem = await _context.Cases
                .Include(c => c.CaseManager)
                .Include(c => c.Notes)
                .FirstOrDefaultAsync(c => c.Id == caseId);
            if (caseItem == null)
            {
                return NotFound();
            }
            return View(caseItem);
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
                    CaseNumber = CaseNumberGenerator.Generate(),
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
            ViewBag.clientId = caseItem.ClientId;
            return View(caseItem);
        }
        [HttpGet]
        public IActionResult CreateNote(int caseId)
        {
            var note = new CreateNoteViewModel
            {
                CaseId = caseId
            };

            return View(note);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateNote(CreateNoteViewModel newNote)
        {

            var currentUser = await _userManager.GetUserAsync(User);
                if (currentUser == null)
                {
                    return Unauthorized();
                }
                if (ModelState.IsValid)
                {
                    var note = new Note
                    {
                        Name = newNote.Name,
                        Text = newNote.Text,
                        CreatedAt = DateTime.UtcNow,
                        CaseId = newNote.CaseId,
                        UserId = currentUser.Id,
                    };

                    _context.Add(note);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(
                        "CaseDetails",
                        "ClientsList",
                        new { area = "CaseManager", caseId = note.CaseId }
                    );
                }
                
                return View(newNote);
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

       
    }
}
