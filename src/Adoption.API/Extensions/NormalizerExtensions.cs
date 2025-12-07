namespace Adoption.API.Extensions;

public static class NormalizerExtensions
{
    public static string NormalizeVaccines(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        var separators = new char[] { ',', ' ' };
        var parts = input
            .Split(separators, StringSplitOptions.RemoveEmptyEntries)
            .Select(p => Capitalize(p.Trim()))
            .Distinct();

        return string.Join(", ", parts);
    }

    private static string Capitalize(string s)
    {
        if (string.IsNullOrEmpty(s)) return s;
        return char.ToUpper(s[0]) + s.Substring(1).ToLower();
    }
    
    public static string CapitalizeFirstWord(this string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        input = input.Trim();
        return char.ToUpper(input[0]) + input.Substring(1).ToLower();
    }

}
