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
            await SeedNotesAsync(db, userManager);
            await SeedCaseHistoriesAsync(db, userManager);
        }

        private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            var roles = Enum.GetNames<UserRole>();

            foreach (var roleName in roles)
            {
                if (await roleManager.RoleExistsAsync(roleName)) continue;
                var result = await roleManager.CreateAsync(
                    new IdentityRole(roleName));
                if (result.Succeeded) continue;
                var errors = string.Join(", ", result.Errors.Select(error => error.Description));
                throw new InvalidOperationException(
                    $"Could not create role '{roleName}': {errors}");
            }
        }

        private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
        {
            await CreateUserAsync(
                userManager,
                "admin@example.com",
                "Jan",
                "Nilsson",
                UserRole.admin
            );

            await CreateUserAsync(
                userManager,
                "client@example.com",
                "Anna",
                "Olsson",
                UserRole.client
                );

            await CreateUserAsync(
                userManager,
                "manager@example.com",
                "Kalle",
                "Jönsson",
                UserRole.caseManager);
        }
        private static async Task CreateUserAsync(
            UserManager<ApplicationUser> userManager,
            string email,
            string firstName,
            string lastName,
            UserRole role)
        {
            var existingUser =
                await userManager.FindByEmailAsync(email);

            if (existingUser != null)
            {
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

            var result = await userManager.CreateAsync(
                user,
                "TestPassword123!");

            if (!result.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    result.Errors.Select(
                        error => error.Description));

                throw new InvalidOperationException(
                    $"Could not create user '{email}': {errors}");
            }

            var roleResult = await userManager.AddToRoleAsync(
                user,
                role.ToString());

            if (!roleResult.Succeeded)
            {
                var errors = string.Join(
                    ", ",
                    roleResult.Errors.Select(
                        error => error.Description));

                throw new InvalidOperationException(
                    $"Could not add user '{email}' to role '{role}': {errors}");
            }
        }


        private static async Task SeedCasesAsync(ApplicationDbContext db, UserManager<ApplicationUser> userManager)
        {
            var client = await userManager.FindByEmailAsync(
                "client@example.com");

            var caseManager = await userManager.FindByEmailAsync(
                "manager@example.com");

            if (client == null || caseManager == null)
            {
                throw new InvalidOperationException(
                    "Could not find seeded client or case manager.");
            }

            // Don't create the seed cases more than once.
            if (await db.Cases.AnyAsync())
            {
                return;
            }

            var now = DateTime.UtcNow;

            var case1 = new Case
            {
                ClientId = client.Id,
                CaseManagerId = caseManager.Id,
                CaseNumber = CaseNumberGenerator.Generate(),
                Title = "Application for Support",
                Description = "The client needs assistance with their application for support.",
                Status = Status.active,
                CreatedDate = now.AddDays(-10),
                UpdatedDate = now.AddDays(-2)
            };

            var case2 = new Case
            {
                ClientId = client.Id,
                CaseManagerId = caseManager.Id,
                CaseNumber = CaseNumberGenerator.Generate(),
                Title = "Follow-up on Previous Case",
                Description = "Follow-up on the client's previous case.",
                Status = Status.waiting,
                CreatedDate = now.AddDays(-5),
                UpdatedDate = now.AddDays(-1)
            };

            var case3 = new Case
            {
                ClientId = client.Id,
                CaseManagerId = caseManager.Id,
                CaseNumber = CaseNumberGenerator.Generate(),
                Title = "Case Closure",
                Description = "The case has been fully processed and can be closed.",
                Status = Status.closed,
                CreatedDate = now.AddDays(-30),
                UpdatedDate = now.AddDays(-15)
            };

            db.Cases.AddRange(case1, case2, case3);

            await db.SaveChangesAsync();
        }


        private static async Task SeedNotesAsync(ApplicationDbContext db, UserManager<ApplicationUser> userManager )
        {
            var caseManager = await userManager.FindByEmailAsync(
                "manager@example.com");

            if (caseManager == null)
            {
                throw new InvalidOperationException(
                    "Could not find seeded case manager.");
            }

            if (await db.Notes.AnyAsync())
            {
                return;
            }

            var case1 = await db.Cases
                .FirstOrDefaultAsync(c =>
                    c.Title == "Application for Support");

            var case2 = await db.Cases
                .FirstOrDefaultAsync(c =>
                    c.Title == "Follow-up on Previous Case");

            if (case1 == null || case2 == null)
            {
                throw new InvalidOperationException(
                    "Could not find seeded cases.");
            }

            var now = DateTime.UtcNow;

            var notes = new List<Note>
        {
        new Note
        {
            Name = "Initial contact",
            Text = "Initial contact with the client regarding the case.",
            CreatedAt = now.AddDays(-9),
            CaseId = case1.Id,
            UserId = caseManager.Id
        },

        new Note
        {
            Name = "Follow-up",
            Text = "The client has submitted additional information.",
            CreatedAt = now.AddDays(-3),
            CaseId = case1.Id,
            UserId = caseManager.Id
        },

        new Note
        {
            Name = "Contact with client",
            Text = "The client was contacted for follow-up.",
            CreatedAt = now.AddDays(-2),
            CaseId = case2.Id,
            UserId = caseManager.Id
        }
    };

            db.Notes.AddRange(notes);

            await db.SaveChangesAsync();
        }


        private static async Task SeedCaseHistoriesAsync(
            ApplicationDbContext db,
            UserManager<ApplicationUser> userManager)
        {
            var caseManager = await userManager.FindByEmailAsync(
                "manager@example.com");

            if (caseManager == null)
            {
                throw new InvalidOperationException(
                    "Could not find seeded case manager.");
            }

            if (await db.CaseHistories.AnyAsync())
            {
                return;
            }

            var case1 = await db.Cases
                .FirstOrDefaultAsync(c =>
                    c.Title == "Application for Support");

            var case2 = await db.Cases
                .FirstOrDefaultAsync(c =>
                    c.Title == "Follow-up on Previous Case");

            var case3 = await db.Cases
                .FirstOrDefaultAsync(c =>
                    c.Title == "Case Closure");

            if (case1 == null || case2 == null || case3 == null)
            {
                throw new InvalidOperationException(
                    "Could not find seeded cases.");
            }

            var histories = new List<CaseHistory>
        {
        // Case 1 created
        new CaseHistory
        {
            CaseId = case1.Id,
            UserId = caseManager.Id,
            Type = CaseHistoryType.CaseCreated,
            OldValue = null,
            NewValue = case1.CaseNumber,
            CreatedDate = case1.CreatedDate
        },

        // Case 1 status changed
        new CaseHistory
        {
            CaseId = case1.Id,
            UserId = caseManager.Id,
            Type = CaseHistoryType.StatusChanged,
            OldValue = Status.waiting.ToString(),
            NewValue = Status.active.ToString(),
            CreatedDate = case1.UpdatedDate
        },

        // Case 2 created
        new CaseHistory
        {
            CaseId = case2.Id,
            UserId = caseManager.Id,
            Type = CaseHistoryType.CaseCreated,
            OldValue = null,
            NewValue = case2.CaseNumber,
            CreatedDate = case2.CreatedDate
        },

        // Case 3 created
        new CaseHistory
        {
            CaseId = case3.Id,
            UserId = caseManager.Id,
            Type = CaseHistoryType.CaseCreated,
            OldValue = null,
            NewValue = case3.CaseNumber,
            CreatedDate = case3.CreatedDate
        }
    };

            db.CaseHistories.AddRange(histories);

            await db.SaveChangesAsync();
        }
    }
}
