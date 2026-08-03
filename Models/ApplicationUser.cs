using System;
using Microsoft.AspNetCore.Identity;

namespace caseManageMentSystem.Models;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public string FullName => $"{FirstName} {LastName}";

    // Ärenden där användaren är klient
    public ICollection<Case> ClientCases { get; set; } = new List<Case>();

    // Ärenden där användaren är handläggare
    public ICollection<Case> ManagedCases { get; set; } = new List<Case>();
}
