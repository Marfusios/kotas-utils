namespace Kotas.Utils.Common.Utils
{
    public static class StringUtils
    {
        public static string ToUpperFirstLetter(this string str)
        {
            if (string.IsNullOrWhiteSpace(str) || str.Length < 1)
                return str;

            if (str.Length > 1)
                return char.ToUpper(str[0]) + str.Substring(1);

            return str.ToUpper();
        }

        public static string ToLowerFirstLetter(this string str)
        {
            if (string.IsNullOrWhiteSpace(str) || str.Length < 1)
                return str;

            if (str.Length > 1)
                return char.ToLower(str[0]) + str.Substring(1);

            return str.ToLower();
        }

        public static int CountSubstring(this string src, string substring)
        {
            return (src.Length - src.Replace(substring, "").Length) / substring.Length;
        }
    }
}
