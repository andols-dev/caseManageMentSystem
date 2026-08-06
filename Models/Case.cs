using caseManageMentSystem.Areas.CaseManager.Enums;

namespace caseManageMentSystem.Models
{
    public class Case
    {
        public int Id { get; set; }

        public string ClientId { get; set; } = string.Empty;
        public ApplicationUser Client { get; set; } = null!;

        public string CaseManagerId { get; set; } = string.Empty;
        public ApplicationUser CaseManager { get; set; } = null!;


        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Status Status { get; set; }  

        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }

        // one case can have many notes 
        public ICollection<Note> Notes { get; set; } = new List<Note>();
    }
}
