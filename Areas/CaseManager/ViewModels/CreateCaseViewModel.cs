using caseManageMentSystem.Areas.CaseManager.Enums;
using caseManageMentSystem.Models;

namespace caseManageMentSystem.Areas.CaseManager.ViewModels
{
    public class CreateCaseViewModel
    {
        public string ClientId { get; set; } = string.Empty;

        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

      

    }
}
