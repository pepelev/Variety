namespace Variety;

public static class StringExtensions
{
    public static string ToLowerFirstCharacter(this string str)
    {
        if (string.IsNullOrEmpty(str) || char.IsLower(str, 0))
        {
            return str;
        }

        if (str.Length == 1)
        {
            return str.ToLowerInvariant();
        }

        return char.IsSurrogatePair(str, 0)
            ? str.Substring(0, 2).ToLowerInvariant() + str.Substring(2)
            : str.Substring(0, 1).ToLowerInvariant() + str.Substring(1);
    }
}