using System;
using System.ComponentModel.DataAnnotations;

namespace caseManageMentSystem.Models;

public class RegisterUser
{
    [Required(ErrorMessage = "Please enter first name.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please enter last name.")]
    public string LastName { get; set; } = string.Empty;
    [Required(ErrorMessage = "Please enter email."), DataType(DataType.EmailAddress)]
    public string Email { get; set; } = string.Empty;
    [Required(ErrorMessage = "Please enter password."), DataType(DataType.Password)]


    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Please confirm your password.")]
    [Compare("Password", ErrorMessage = "Passwords do not match.")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
