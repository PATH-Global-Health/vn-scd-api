using System;
using System.Collections.Generic;
using System.Text;

namespace Data.Models.CustomModels
{
    public static class CustomFormating
    {
        /// <summary>
        /// To 'dd/MM/yyyy'
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToVNDate(this DateTime dt)
        {
            return dt.ToString("dd/MM/yyyy");
        }

        /// <summary>
        /// To 'MM/yyyy'
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string ToReportingPeriod(this DateTime dt)
        {
            return dt.ToString("MM/yyyy");
        }

        public static DateTime ToReportDatetime(this DateTime date)
        {
            return new DateTime(date.Year, date.Month, DateTime.DaysInMonth(date.Year, date.Month));
        }
    }
}
