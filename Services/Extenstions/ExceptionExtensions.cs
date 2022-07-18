using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Services.Extenstions
{
    public static class ExceptionExtensions
    {
        public static string ReturnDuplicateMessage(this SqlException sqle)
        {
             var arr = sqle.Message.SplitByApostrophe();
             var arr1 = sqle.Message.SplitByParentheses();
            return $"\'{arr[3]}\' has duplicate value : \'{arr1[1]}\'";
        }

        public static bool IsDuplicateKeyException(this SqlException sqle)
        {
            return sqle.Number == 2601;
        }
    }
}
