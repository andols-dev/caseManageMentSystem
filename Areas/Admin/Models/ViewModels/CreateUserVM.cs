using System;
using System.ComponentModel.DataAnnotations;
using caseManageMentSystem.Enums;

namespace caseManageMentSystem.Areas.Admin.Models;

public class CreateUserVM
{
    [Required(ErrorMessage = "Please enter first name.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter last name.")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter email."), DataType(DataType.EmailAddress)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter password."), DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please select a role.")]
    public UserRole Role { get; set; }
}
