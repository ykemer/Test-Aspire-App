namespace Aspire_App.Web.Helpers;

public static class FrontendHelper
{
    public static string ToPascalCase(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        return char.ToUpper(input[0]) + input.Substring(1);
    }
}