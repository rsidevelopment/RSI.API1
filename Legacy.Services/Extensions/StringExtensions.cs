using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Text;

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
    }
}
