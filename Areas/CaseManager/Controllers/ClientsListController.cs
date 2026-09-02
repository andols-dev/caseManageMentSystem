using caseManageMentSystem.Areas.CaseManager.Services;
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
    public class ClientsListController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, INoteService noteService, ICaseCaseManagerService caseService, ICaseHistoryService caseHistoryService) : Controller
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
                return Unauthorized();

            if (!ModelState.IsValid)
                return View(caseItem);
            
            // Create case
            var newCase = await caseService.CreateCase(caseItem, currentUser);
            
            // Create case history
            await caseHistoryService.CreateCaseHistory(newCase.Id, currentUser);
            
            return RedirectToAction("Index", "DashBoard");
            
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

                if (!ModelState.IsValid) return View(newNote);
                await noteService.CreateNote(newNote, currentUser);

                return RedirectToAction(
                    "CaseDetails",
                    "ClientsList",
                    new { area = "CaseManager", caseId = newNote.CaseId }
                );
        }
    }
}
