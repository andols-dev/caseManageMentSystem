using System;

namespace caseManageMentSystem.Areas.Admin.Models.ViewModels;

public class EditUserVM
{
    public string? Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
