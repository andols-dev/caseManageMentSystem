
using caseManageMentSystem.Areas.Client.Services;
using caseManageMentSystem.Areas.Client.ViewModels;
using caseManageMentSystem.Data;
using caseManageMentSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace caseManageMentSystem.Areas.Client.Controllers
{
    [Area("Client")]
    [Authorize(Roles = "client")]
    public class DashBoardController(
        UserManager<ApplicationUser> userManager,
        ICaseService caseService,
        ApplicationDbContext context,
        IWebHostEnvironment environment) : Controller
    {
        private const long MaxFileSize = 10 * 1024 * 1024;
        private const int MaxFiles = 5;

        private static readonly Dictionary<string, string> AllowedFiles = new()
        {
            { ".pdf", "application/pdf" },
            { ".doc", "application/msword" },
            { ".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document" },
            { ".xls", "application/vnd.ms-excel" },
            { ".xlsx", "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" },
            { ".jpg", "image/jpeg" },
            { ".jpeg", "image/jpeg" },
            { ".png", "image/png" }
        };

        public async Task<IActionResult> Index()
        {
            var user = await userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            var cases = await caseService.GetCasesForClient(user.Id);

            var viewModel = new DashBoardViewModel(
                user.FullName,
                cases
            );

            return View(viewModel);
        }

        // Show case details
        public async Task<IActionResult> Details(int id)
        {
            var user = await userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            var caseItem = await context.Cases
                .Include(c => c.CaseManager)
                .Include(c => c.Notes)
                .Include(c => c.Documents)
                .FirstOrDefaultAsync(c =>
                    c.Id == id &&
                    c.ClientId == user.Id);

            if (caseItem == null)
                return NotFound();

            return View(caseItem);
        }

        // Display upload page
        [HttpGet]
        public async Task<IActionResult> UploadDocument(int caseId)
        {
            var user = await userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            var caseItem = await context.Cases
                .FirstOrDefaultAsync(c =>
                    c.Id == caseId &&
                    c.ClientId == user.Id);

            if (caseItem == null)
                return NotFound();

            return View(caseItem);
        }

        // Upload documents
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadDocument(
            int caseId,
            List<IFormFile> files)
        {
            var user = await userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            var caseItem = await context.Cases
                .FirstOrDefaultAsync(c =>
                    c.Id == caseId &&
                    c.ClientId == user.Id);

            if (caseItem == null)
                return NotFound();

            if (files == null || files.Count == 0)
            {
                ModelState.AddModelError("files", "Please select at least one document.");
                return View(caseItem);
            }

            if (files.Count > MaxFiles)
            {
                ModelState.AddModelError("files", $"You can upload a maximum of {MaxFiles} files.");
                return View(caseItem);
            }

            // Validate all files before saving
            foreach (var file in files)
            {
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

                if (!AllowedFiles.ContainsKey(extension))
                {
                    ModelState.AddModelError("files",
                        $"File type {extension} is not allowed.");
                }

                if (file.Length == 0 || file.Length > MaxFileSize)
                {
                    ModelState.AddModelError("files",
                        "Each file must be larger than 0 bytes and smaller than 10 MB.");
                }
            }

            if (!ModelState.IsValid)
                return View(caseItem);

            var uploadDirectory = Path.Combine(
                environment.ContentRootPath,
                "App_Data",
                "CaseDocuments",
                caseId.ToString());

            Directory.CreateDirectory(uploadDirectory);

            foreach (var file in files)
            {
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

                var storedFileName = $"{Guid.NewGuid():N}{extension}";

                var filePath = Path.Combine(
                    uploadDirectory,
                    storedFileName);

                await using (var stream = new FileStream(
                    filePath,
                    FileMode.CreateNew,
                    FileAccess.Write))
                {
                    await file.CopyToAsync(stream);
                }

                var document = new CaseDocument
                {
                    FileName = Path.GetFileName(file.FileName),
                    StoredFileName = storedFileName,
                    ContentType = AllowedFiles[extension],
                    FileSize = file.Length,
                    UploadedAt = DateTime.UtcNow,
                    CaseId = caseId
                };

                context.CaseDocuments.Add(document);
            }

            await context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Documents uploaded successfully.";

            return RedirectToAction(nameof(Details), new { id = caseId });
        }

        // Download document
        [HttpGet]
        public async Task<IActionResult> DownloadDocument(int documentId)
        {
            var user = await userManager.GetUserAsync(User);

            if (user == null)
                return NotFound();

            var document = await context.CaseDocuments
                .Include(d => d.Case)
                .FirstOrDefaultAsync(d =>
                    d.Id == documentId &&
                    d.Case.ClientId == user.Id);

            if (document == null)
                return NotFound();

            var filePath = Path.Combine(
                environment.ContentRootPath,
                "App_Data",
                "CaseDocuments",
                document.CaseId.ToString(),
                document.StoredFileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound();

            var stream = new FileStream(
                filePath,
                FileMode.Open,
                FileAccess.Read);

            return File(
                stream,
                document.ContentType,
                document.FileName);
        }
    }
}
