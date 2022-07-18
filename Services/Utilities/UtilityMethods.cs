using System;
using System.Collections.Generic;
using System.Text;

namespace Services
{
    public static class UtilityMethods
    {
        public static double? Percentage(double? value1, double? value2)
        {
            if (value1 == null || value2 == null)
                return null;
            return Math.Round(value1.Value / value2.Value, 4, MidpointRounding.AwayFromZero);
        }

        public static double Percentage(double value1, double value2)
        {
            return Math.Round(value1 / value2, 4, MidpointRounding.AwayFromZero);
        }

        public static double Percentage(int value1, int value2)
        {
            return Math.Round((double)value1 / (double)value2, 4, MidpointRounding.AwayFromZero);
        }

        public static IEnumerable<DateTime> ToReportDateTime(this IEnumerable<DateTime> dateTimes)
        {
            foreach (var item in dateTimes)
            {
                yield return new DateTime(item.Year, item.Month, DateTime.DaysInMonth(item.Year, item.Month));
            }
        }

        //public static bool IsOverlap(DateTime start1, DateTime end1, DateTime start2, DateTime end2)
        //{
        //    if (end2 >= start1 || start2 <= end1)
        //        return true;
        //    return false;
        //}
    }
}
