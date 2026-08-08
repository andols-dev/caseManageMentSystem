using caseManageMentSystem.Models;

namespace caseManageMentSystem.Areas.CaseManager.Enums
{
    public class CasesViewModel
    {
        public List<Case> Cases { get; set; } = [];

        public string FullName { get; set; } = string.Empty;

        public Status? CaseStatus { get; set; }
    }
}
 