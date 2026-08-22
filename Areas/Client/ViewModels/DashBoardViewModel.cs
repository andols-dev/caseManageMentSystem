namespace caseManageMentSystem.Areas.Client.ViewModels;

public record DashBoardViewModel(
    string FullName,
    IReadOnlyList<CaseListItemViewModel> Cases
    );