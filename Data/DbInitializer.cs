
using caseManageMentSystem.Areas.CaseManager.Enums;
using caseManageMentSystem.Enums;
using caseManageMentSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using caseManageMentSystem.Services;

namespace caseManageMentSystem.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            var db = services.GetRequiredService<ApplicationDbContext>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            await db.Database.MigrateAsync();

            await SeedRolesAsync(roleManager);
            await SeedUsersAsync(userManager);
            await SeedCasesAsync(db, userManager);
            await SeedNotesAsync(db);
            await SeedCaseHistoriesAsync(db);
        }

        // SEED ROLES
        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            var roles = Enum.GetNames<UserRole>();

            foreach (var roleName in roles)
            {
                if (await roleManager.RoleExistsAsync(roleName))
                    continue;

                var result = await roleManager.CreateAsync(new IdentityRole(roleName));

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new InvalidOperationException($"Could not create role '{roleName}': {errors}");
                }
            }
        }

        // SEED USERS
        private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
        {
            await CreateUserAsync(userManager, "admin@example.com", "Jan", "Nilsson", UserRole.admin);

            var managers = new[]
            {
                ("kalle@example.com", "Kalle", "Jönsson"),
                ("maria@example.com", "Maria", "Andersson"),
                ("erik@example.com", "Erik", "Karlsson"),
                ("lisa@example.com", "Lisa", "Svensson"),
                ("anna.manager@example.com", "Anna", "Bergström")
            };

            foreach (var manager in managers)
            {
                await CreateUserAsync(userManager, manager.Item1, manager.Item2, manager.Item3, UserRole.caseManager);
            }

            var clients = new[]
            {
                ("klient1@example.com", "Erik", "Johansson"),
                ("klient2@example.com", "Sara", "Lindberg"),
                ("klient3@example.com", "Oskar", "Persson"),
                ("klient4@example.com", "Emma", "Lundgren"),
                ("klient5@example.com", "David", "Holm"),
                ("klient6@example.com", "Sofia", "Björk"),
                ("klient7@example.com", "Anders", "Wallin"),
                ("klient8@example.com", "Maja", "Ekström"),
                ("klient9@example.com", "Filip", "Berg"),
                ("klient10@example.com", "Elin", "Ström")
            };

            foreach (var client in clients)
            {
                await CreateUserAsync(userManager, client.Item1, client.Item2, client.Item3, UserRole.client);
            }
        }

        private static async Task CreateUserAsync(
            UserManager<ApplicationUser> userManager,
            string email,
            string firstName,
            string lastName,
            UserRole role)
        {
            var existingUser = await userManager.FindByEmailAsync(email);

            if (existingUser != null)
            {
                if (!await userManager.IsInRoleAsync(existingUser, role.ToString()))
                {
                    var roleResult = await userManager.AddToRoleAsync(existingUser, role.ToString());

                    if (!roleResult.Succeeded)
                    {
                        var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                        throw new InvalidOperationException($"Could not add role to '{email}': {errors}");
                    }
                }

                return;
            }

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FirstName = firstName,
                LastName = lastName
            };

            var result = await userManager.CreateAsync(user, "TestPassword123!");

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Could not create user '{email}': {errors}");
            }

            var addRoleResult = await userManager.AddToRoleAsync(user, role.ToString());

            if (!addRoleResult.Succeeded)
            {
                var errors = string.Join(", ", addRoleResult.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Could not add role to '{email}': {errors}");
            }
        }

        // SEED CASES
        private static async Task SeedCasesAsync(
            ApplicationDbContext db,
            UserManager<ApplicationUser> userManager)
        {
            var managerEmails = new[]
            {
                "kalle@example.com",
                "maria@example.com",
                "erik@example.com",
                "lisa@example.com",
                "anna.manager@example.com"
            };

            var clientEmails = new[]
            {
                "klient1@example.com",
                "klient2@example.com",
                "klient3@example.com",
                "klient4@example.com",
                "klient5@example.com",
                "klient6@example.com",
                "klient7@example.com",
                "klient8@example.com",
                "klient9@example.com",
                "klient10@example.com"
            };

            var managers = new List<ApplicationUser>();
            var clients = new List<ApplicationUser>();

            foreach (var email in managerEmails)
            {
                var manager = await userManager.FindByEmailAsync(email);

                if (manager == null)
                    throw new InvalidOperationException($"Could not find manager: {email}");

                managers.Add(manager);
            }

            foreach (var email in clientEmails)
            {
                var client = await userManager.FindByEmailAsync(email);

                if (client == null)
                    throw new InvalidOperationException($"Could not find client: {email}");

                clients.Add(client);
            }

            // Skapa bara ärenden som saknas
            var now = DateTime.UtcNow;

            var cases = new List<Case>
            {
                new Case
                {
                    ClientId = clients[0].Id,
                    CaseManagerId = managers[0].Id,
                    CaseNumber = CaseNumberGenerator.Generate(),
                    Title = "Lorem ipsum dolor sit amet",
                    Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
                    Status = Status.active,
                    CreatedDate = now.AddDays(-10),
                    UpdatedDate = now.AddDays(-2)
                },
                new Case
                {
                    ClientId = clients[1].Id,
                    CaseManagerId = managers[0].Id,
                    CaseNumber = CaseNumberGenerator.Generate(),
                    Title = "Consectetur adipiscing elit",
                    Description = "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
                    Status = Status.waiting,
                    CreatedDate = now.AddDays(-8),
                    UpdatedDate = now.AddDays(-3)
                },
                new Case
                {
                    ClientId = clients[2].Id,
                    CaseManagerId = managers[1].Id,
                    CaseNumber = CaseNumberGenerator.Generate(),
                    Title = "Sed do eiusmod tempor",
                    Description = "Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur.",
                    Status = Status.active,
                    CreatedDate = now.AddDays(-12),
                    UpdatedDate = now.AddDays(-1)
                },
                new Case
                {
                    ClientId = clients[3].Id,
                    CaseManagerId = managers[1].Id,
                    CaseNumber = CaseNumberGenerator.Generate(),
                    Title = "Ut enim ad minim veniam",
                    Description = "Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
                    Status = Status.waiting,
                    CreatedDate = now.AddDays(-7),
                    UpdatedDate = now.AddDays(-2)
                },
                new Case
                {
                    ClientId = clients[4].Id,
                    CaseManagerId = managers[2].Id,
                    CaseNumber = CaseNumberGenerator.Generate(),
                    Title = "Quis nostrud exercitation",
                    Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer nec odio. Praesent libero.",
                    Status = Status.active,
                    CreatedDate = now.AddDays(-15),
                    UpdatedDate = now.AddDays(-4)
                },
                new Case
                {
                    ClientId = clients[5].Id,
                    CaseManagerId = managers[2].Id,
                    CaseNumber = CaseNumberGenerator.Generate(),
                    Title = "Duis aute irure dolor",
                    Description = "Sed cursus ante dapibus diam. Sed nisi. Nulla quis sem at nibh elementum imperdiet.",
                    Status = Status.waiting,
                    CreatedDate = now.AddDays(-6),
                    UpdatedDate = now.AddDays(-1)
                },
                new Case
                {
                    ClientId = clients[6].Id,
                    CaseManagerId = managers[3].Id,
                    CaseNumber = CaseNumberGenerator.Generate(),
                    Title = "Excepteur sint occaecat",
                    Description = "Duis sagittis ipsum. Praesent mauris. Fusce nec tellus sed augue semper porta.",
                    Status = Status.active,
                    CreatedDate = now.AddDays(-20),
                    UpdatedDate = now.AddDays(-5)
                },
                new Case
                {
                    ClientId = clients[7].Id,
                    CaseManagerId = managers[3].Id,
                    CaseNumber = CaseNumberGenerator.Generate(),
                    Title = "Integer nec odio",
                    Description = "Mauris massa. Vestibulum lacinia arcu eget nulla. Class aptent taciti sociosqu ad litora torquent.",
                    Status = Status.closed,
                    CreatedDate = now.AddDays(-30),
                    UpdatedDate = now.AddDays(-10)
                },
                new Case
                {
                    ClientId = clients[8].Id,
                    CaseManagerId = managers[4].Id,
                    CaseNumber = CaseNumberGenerator.Generate(),
                    Title = "Praesent libero",
                    Description = "Curabitur sodales ligula in libero. Sed dignissim lacinia nunc. Curabitur tortor.",
                    Status = Status.active,
                    CreatedDate = now.AddDays(-9),
                    UpdatedDate = now.AddDays(-2)
                },
                new Case
                {
                    ClientId = clients[9].Id,
                    CaseManagerId = managers[4].Id,
                    CaseNumber = CaseNumberGenerator.Generate(),
                    Title = "Vestibulum lacinia arcu",
                    Description = "Pellentesque nibh. Aenean quam. In scelerisque sem at dolor. Maecenas mattis.",
                    Status = Status.waiting,
                    CreatedDate = now.AddDays(-5),
                    UpdatedDate = now.AddDays(-1)
                }
            };

            foreach (var caseItem in cases)
            {
                var exists = await db.Cases.AnyAsync(c =>
                    c.ClientId == caseItem.ClientId);

                if (!exists)
                    db.Cases.Add(caseItem);
            }

            await db.SaveChangesAsync();
        }

        // SEED NOTES
        private static async Task SeedNotesAsync(ApplicationDbContext db)
        {
            var cases = await db.Cases.ToListAsync();

            foreach (var caseItem in cases)
            {
                var hasNotes = await db.Notes.AnyAsync(n => n.CaseId == caseItem.Id);

                if (hasNotes)
                    continue;

                db.Notes.AddRange(
                    new Note
                    {
                        Name = "Lorem ipsum dolor sit amet",
                        Text = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.",
                        CreatedAt = caseItem.CreatedDate.AddDays(1),
                        CaseId = caseItem.Id,
                        UserId = caseItem.CaseManagerId
                    },
                    new Note
                    {
                        Name = "Consectetur adipiscing elit",
                        Text = "Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat.",
                        CreatedAt = caseItem.UpdatedDate,
                        CaseId = caseItem.Id,
                        UserId = caseItem.CaseManagerId
                    }
                );
            }

            await db.SaveChangesAsync();
        }

        // SEED CASE HISTORIES
        private static async Task SeedCaseHistoriesAsync(ApplicationDbContext db)
        {
            var cases = await db.Cases.ToListAsync();

            foreach (var caseItem in cases)
            {
                var hasHistory = await db.CaseHistories.AnyAsync(h => h.CaseId == caseItem.Id);

                if (hasHistory)
                    continue;

                var histories = new List<CaseHistory>
                {
                    new CaseHistory
                    {
                        CaseId = caseItem.Id,
                        UserId = caseItem.CaseManagerId,
                        Type = CaseHistoryType.CaseCreated,
                        OldValue = null,
                        NewValue = caseItem.CaseNumber,
                        CreatedDate = caseItem.CreatedDate
                    },
                    new CaseHistory
                    {
                        CaseId = caseItem.Id,
                        UserId = caseItem.CaseManagerId,
                        Type = CaseHistoryType.StatusChanged,
                        OldValue = Status.waiting.ToString(),
                        NewValue = caseItem.Status.ToString(),
                        CreatedDate = caseItem.UpdatedDate
                    }
                };

                db.CaseHistories.AddRange(histories);
            }

            await db.SaveChangesAsync();
        }
    }
}
