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
    public class ClientsListController(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        INoteService noteService,
        ICaseCaseManagerService caseService,
        ICaseHistoryService caseHistoryService,
        IClientService clientService)  : Controller
    {
        [HttpGet]
        public async Task<IActionResult> Index(string? search, int pageNumber = 1)
        {
            const int pageSize = 10;

            // Get clients belonging to the client role
            var clientsQuery = context.Users.Where(user =>
                context.UserRoles.Any(userRole =>
                    userRole.UserId == user.Id &&
                    context.Roles.Any(role =>
                        role.Id == userRole.RoleId &&
                        role.NormalizedName == "CLIENT"
                    )
                )
            );

            // Search by first name, last name or email
            if (!string.IsNullOrWhiteSpace(search))
            {
                search = search.Trim();
                var searchLower = search.ToLower();

                clientsQuery = clientsQuery.Where(user =>
                    (user.FirstName != null &&
                     user.FirstName.ToLower().Contains(searchLower)) ||

                    (user.LastName != null &&
                     user.LastName.ToLower().Contains(searchLower)) ||

                    (user.Email != null &&
                     user.Email.ToLower().Contains(searchLower))
                );
            }

            // Get total number of matching clients
            var totalItems = await clientsQuery.CountAsync();

            // Calculate pagination
            var totalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            pageNumber = Math.Max(1, pageNumber);

            if (totalPages > 0)
                pageNumber = Math.Min(pageNumber, totalPages);

            // Get clients for current page
            var clients = await clientsQuery
                .OrderBy(user => user.LastName)
                .ThenBy(user => user.FirstName)
                .ThenBy(user => user.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Create paged result
            var result = new PagedResult<ApplicationUser>
            {
                Items = clients,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalItems = totalItems
            };

            ViewBag.Search = search;

            return View(result);
        }

        /*public async Task<IActionResult> Index()
        {
            var clients = await userManager.GetUsersInRoleAsync("client");
            return View(clients);
        }*/

        // GET: ClientsListController/Details/5
        public async Task<IActionResult> Details(string id)
        {
            var clientAndCases = await clientService.GetClientWithCases(id);

            return clientAndCases == null ? NotFound() : View(clientAndCases);
        }

        // GET: ClientsListController/CaseDetails/5
        public async Task<IActionResult> CaseDetails(int caseId)
        {
            var caseItem = await caseService.GetCaseDetails(caseId);
            return caseItem == null ? NotFound() : View(caseItem);
        }

        // GET: ClientsListController/Create
        public ActionResult Create(string clientId)
        {
            var model = new CreateCaseViewModel()
            {
                ClientId = clientId
            };
            return View(model);
        }

        // POST: ClientsListController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(CreateCaseViewModel caseItem)
        {
            var currentUser = await userManager.GetUserAsync(User);
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
            var currentUser = await userManager.GetUserAsync(User);
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
