using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace caseManageMentSystem.Models
{
    public class Note
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public string Text { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public int CaseId { get; set; }

        [ValidateNever]
        public Case Case { get; set; } = null!;

        public string UserId { get; set; } = string.Empty;
        public ApplicationUser User { get; set; } = null!;
    }
}
