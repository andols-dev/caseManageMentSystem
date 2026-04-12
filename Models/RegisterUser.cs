using System;
using System.ComponentModel.DataAnnotations;

namespace caseManageMentSystem.Models;

public class RegisterUser
{
    [Required(ErrorMessage = "Please enter email."),DataType(DataType.EmailAddress)]

    public string Email { get; set; }
    [Required(ErrorMessage = "Please enter password."), DataType(DataType.Password)]
    public string Password { get; set; }
}
