using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Services.Extenstions
{
    public static class UtilityExtensions
    {
        public static readonly CultureInfo cultureInfo = CultureInfo.GetCultureInfo("vi-VN");
        public static readonly string[] dateTimeFormats
            = new string[] {
                "yyyy",
                "dd/MM/yyyy",
                "dd-MM-yyyy",
                "dd/MM/yyyy HH:mm",
                "dd/MM/yyyy h:mm tt",
                "dd/MM/yyyy hh:mm tt",
                "dd/MM/yyyy hh:mm:ss",
                "dd/MM/yyyy hh:mm:ss tt",
                "dd-MM-yyyy HH:mm",
                "dd-MM-yyyy h:mm tt",
                "dd-MM-yyyy hh:mm tt",
                "dd-MM-yyyy hh:mm:ss",
                "dd-MM-yyyy hh:mm:ss tt",
                "yyyy/MM/dd HH:mm",
                "yyyy/MM/dd",
                "yyyy-MM-dd",
                "yyyy-MM-dd HH:mm",
                "yyyyMMddHHmm"}
            ;

        public static DateTime TryParseDateTime(this string dateS)
        {
            try
            {
                var result = DateTime.ParseExact(dateS, dateTimeFormats, CultureInfo.InvariantCulture);
                return result;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static string[] SplitByApostrophe(this string str)
        {
            //var arr = Regex.Match(str, @"\'([^)]*)\'");
            var arr = str.Split('\'', '\'');
            return arr;
        }

        public static string[] SplitByParentheses(this string str)
        {
            //var arr = Regex.Match(str, @"\'([^)]*)\'");
            var arr = str.Split('(', ')');
            return arr;
        }
    }
}
