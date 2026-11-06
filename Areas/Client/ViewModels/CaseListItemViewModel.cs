using caseManageMentSystem.Areas.CaseManager.Enums;

namespace caseManageMentSystem.Areas.Client.ViewModels;

public record CaseListItemViewModel(
    int Id,
    string CaseNumber,
    string Title, 
    string Description,
    Status status,
    DateTime CreatedAt
    );