using caseManageMentSystem.Data;
using caseManageMentSystem.Models;

namespace caseManageMentSystem.Areas.CaseManager.Services;

public class NoteService(ApplicationDbContext context) : INoteService
{
    
    public async Task CreateNote(CreateNoteViewModel note, ApplicationUser currentUser)
    {
        var newNote = new Note
        {
            Name = note.Name,
            Text = note.Text,
            CreatedAt = DateTime.UtcNow,
            CaseId = note.CaseId,
            UserId = currentUser.Id,
        };

        context.Add(newNote);
        await context.SaveChangesAsync();
    }
}