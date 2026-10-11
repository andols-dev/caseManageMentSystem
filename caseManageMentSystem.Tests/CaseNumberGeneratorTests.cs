using caseManageMentSystem.Services;

namespace caseManageMentSystem.Tests;

public class CaseNumberGeneratorTests
{
    
    [Fact]
    public void Generate_ShouldReturnValidCaseNumber()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        var pattern = $@"^\d{{4}}-[{chars}]{{8}}$";
        var result = CaseNumberGenerator.Generate();
        Assert.Equal(13, result.Length);
        Assert.Matches(pattern, result);
        Assert.StartsWith(DateTime.Now.Year.ToString(), result);
    }
}