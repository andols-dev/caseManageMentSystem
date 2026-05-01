using System;
using System.ComponentModel.DataAnnotations;
using caseManageMentSystem.Enums;

namespace caseManageMentSystem.Areas.Admin.Models;

public class CreateUserVM
{

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;


    public string Password { get; set; } = string.Empty;




    public string Role { get; set; } = string.Empty;
}
