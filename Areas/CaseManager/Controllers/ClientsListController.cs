using caseManageMentSystem.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace caseManageMentSystem.Areas.CaseManager.Controllers
{
    [Area("CaseManager")]
    [Authorize(Roles = "caseManager")]
    public class ClientsListController(ApplicationDbContext context) : Controller
    {
        // GET: ClientsListController
        // Show all clients

        private readonly ApplicationDbContext _context = context;

        public async Task<IActionResult> Index()
        {
            // get all the clients(users)

            var clients = await _context.Users.ToListAsync();
            return View(clients);
        }

        // GET: ClientsListController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: ClientsListController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: ClientsListController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
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
