using caseManageMentSystem.Enums;

namespace caseManageMentSystem.Models
{
    public class CaseHistory
    {
        public int Id { get; set; }

        public int CaseId { get; set; }

        public Case Case { get; set; } = null!;

        public CaseHistoryType Type { get; set; }

        public string UserId { get; set; } = string.Empty;

        public ApplicationUser User { get; set; } = null!;

        public string? OldValue { get; set; }

        public string? NewValue { get; set; }

        public DateTime CreatedDate { get; set; }
    }
}