using caseManageMentSystem.Models;

namespace caseManageMentSystem.Areas.CaseManager.Enums
{
    public class CasesViewModel
    {
        public List<Case> Cases { get; set; } = [];

        public string FullName { get; set; } = string.Empty;

        public Status? CaseStatus { get; set; }

        public string Search { get; set; } = string.Empty;
        public int TotalCases { get; internal set; }
        public int ActiveCases { get; internal set; }
        public int ClosedCases { get; internal set; }
        public int WaitingCases { get; internal set; }
        public int DelayedCases { get; internal set; }
    }
}
 