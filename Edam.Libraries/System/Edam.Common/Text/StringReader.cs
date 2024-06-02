using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Edam.Text
{

    public class StringReader
    {
        private const string NULL = "null";

        public static string GetString(string value)
        {
            if (value == null || value.ToLower() == NULL)
            {
                return null;
            }
            return value;
        }

        public static int? GetInteger(string value)
        {
            if (value == null || value.ToLower() == NULL)
            {
                return null;
            }
            if (int.TryParse(value, out int valueInt))
            {
                return valueInt;
            }
            return null;
        }

        public static long? GetLong(string value)
        {
            if (value == null || value.ToLower() == NULL)
            {
                return null;
            }
            if (long.TryParse(value, out long valueLong))
            {
                return valueLong;
            }
            return null;
        }

        public static bool GetBool(string value)
        {
            string val = value != null && value != NULL ? 
               value.ToLower() : null;
            if (value == null || val == NULL)
            {
                return false;
            }
            return val == "1" || val.StartsWith('t') || val.StartsWith('y');
        }

        public static decimal? GetDecimal(string value)
        {
            if (value == null || value.ToLower() == NULL)
            {
                return null;
            }
            if (decimal.TryParse(value, out decimal valueLong))
            {
                return valueLong;
            }
            return null;
        }
    }

}
