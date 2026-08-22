using caseManageMentSystem.Areas.CaseManager.Enums;

namespace caseManageMentSystem.Areas.Client.ViewModels;

public record CaseListItemViewModel(
    string CaseNumber,
    string Title, 
    string Description,
    Status status,
    DateTime CreatedAt
    );