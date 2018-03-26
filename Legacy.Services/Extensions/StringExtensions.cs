using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Text;
using System.Text.RegularExpressions;

namespace Legacy.Services.Data
{
    public static class StringExtensions
    {
        public static decimal? ToNullDecimal(this string s)
        {
            decimal d = 0;
            if (decimal.TryParse(s, out d))
                return d;
            return null;
        }
        public static int? ToNullInteger(this string s)
        {
            int i = 0;
            if (int.TryParse(s, out i))
                return i;
            return null;
        }
        static string[] splitCamelCase(string s)
        {
            return Regex.Split(s, @"(?<!^)(?=[A-Z])");
        }
        public static string SplitCamelCase(this string s)
        {
            string[] nameParts = splitCamelCase(s);
            if (nameParts.Length == 0) return string.Empty;

            string ret = nameParts[0];
            for (int i = 1; i < nameParts.Length; i++)
            {
                if (nameParts[i - 1].Length > 1)
                    ret += " ";

                ret += nameParts[i];
            }

            return ret;
        }
    }
}
