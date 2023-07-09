using System.Text.RegularExpressions;

namespace AnimeVnInfoBackend.Utilities.Helpers
{
    public static class StringHandle
    {
        public static string RemoveNonAlphanumeric(this string value)
        {
            return RemoveNonAlphanumeric(value, null);
        }

        public static string RemoveNonAlphanumeric(this string value, params char[]? acceptCharacters)
        {
            string pattern = "^a-zA-Z0-9";
            string endPattern = "";
            if (acceptCharacters != null)
            {
                foreach (char c in acceptCharacters)
                {
                    if (pattern.Contains(c))
                    {
                        continue;
                    }
                    if (c == '-')
                    {
                        endPattern = "-";
                        continue;
                    }
                    else if (c == '\\')
                    {
                        pattern += "\\\\";
                    }
                    else
                    {
                        pattern += c;
                    }
                }
            }
            pattern = $"[{pattern}{endPattern}]+";
            return Regex.Replace(value, pattern, string.Empty);
        }

        public static string RemoveLongSpace(this string value)
        {
            return RemoveLongSpace(value, false);
        }

        public static string RemoveLongSpace(this string value, bool hasTrim)
        {
            if (hasTrim)
            {
                value = value.Trim();
            }
            return Regex.Replace(value, "\\s+", " ");
        }
    }
}
