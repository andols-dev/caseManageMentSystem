using System;
using caseManageMentSystem.Models;

namespace caseManageMentSystem.Areas.Admin.Models.ViewModels;

public class UserVM
{
    public ApplicationUser User { get; set; } = new ApplicationUser();

    public IList<string> Roles { get; set; } = [];
}
