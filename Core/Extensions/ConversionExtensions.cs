using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Extensions
{
    public static class ConversionExtensions
    {
        public static int ToInt(this string val, int defaultVal = 0)
        {
            int parsedVal;
            if (int.TryParse(val, out parsedVal))
            {
                return parsedVal;
            }

            return defaultVal;
        }

        public static bool ToBool(this string val, bool defaultVal = false)
        {
            var trueValues = new List<string>(new string[] { "true", "yes", "1" });
            if (string.IsNullOrWhiteSpace(val))
            {
                return defaultVal;
            }
            var lowerVal = val.ToLowerInvariant();

            return trueValues.Contains(lowerVal) ? true : defaultVal;
        }
    }
}
