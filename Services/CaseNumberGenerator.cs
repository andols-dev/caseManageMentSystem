using System.Security.Cryptography;
using System.Text;

namespace caseManageMentSystem.Services
{
    public static class CaseNumberGenerator
    {
        public static string Generate()
        {
            const string chars =
                "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";

            var result = new StringBuilder(13);

            result.Append(DateTime.UtcNow.Year);
            result.Append('-');

            for (int i = 0; i < 8; i++)
            {
                result.Append(
                    chars[RandomNumberGenerator.GetInt32(chars.Length)]);
            }

            return result.ToString();
        }
    }
}
