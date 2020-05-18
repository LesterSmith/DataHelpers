using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataHelpers
{
    public static class Extensions
    {
        public static string GetNotDBNull(this object str)
        {
            return (str == DBNull.Value || str == null) ? string.Empty : str.ToString().Trim();
        }

        public static bool IsNumeric(this string str, out int numeral)
        {
            return int.TryParse(str, out numeral);
        }

        public static bool GetNotDBNullBool(this object str)
        {
            return str != DBNull.Value && Convert.ToBoolean(str);
        }

        public static int GetNotDBNullInt(this object str)
        {
            return str == DBNull.Value ? 0 : (int)str;
        }
    }
}
