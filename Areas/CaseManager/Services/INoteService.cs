using caseManageMentSystem.Models;

namespace caseManageMentSystem.Areas.CaseManager.Services;

public interface INoteService
{
    Task CreateNote(CreateNoteViewModel note, ApplicationUser user);
}