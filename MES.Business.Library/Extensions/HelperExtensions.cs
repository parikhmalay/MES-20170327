using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MES.Business.Library.Extensions
{
    public static class HelperExtensions
    {
        /// <summary>
        /// Formats the date.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        public static string FormatDate(this DateTime? date)
        {
            if (date.HasValue)
                return date.Value.ToString("MM/dd/yyyy");
            else
                return string.Empty;
        }
        /// <summary>
        /// Formats the date time.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        public static string FormatDateTime(this DateTime? date)
        {
            if (date.HasValue)
                return date.Value.ToString("MM/dd/yyyy hh:mm tt");
            else
                return string.Empty;
        }
        /// <summary>
        /// Formats the date.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        public static string FormatDate(this DateTime date)
        {
            return date.ToString("MM/dd/yyyy");
        }
        /// <summary>
        /// Formats the date time.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        public static string FormatDateTime(this DateTime date)
        {
            return date.ToString("MM/dd/yyyy hh:mm tt");
        }
        /// <summary>
        /// Formats the date time in mediam date string with time.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        public static string FormatDateInMediumDateWithTime(this DateTime date)
        {
            return date.ToString("d-MMM-y hh:mm:ss tt");
        }
        /// <summary>
        /// Formats the date time in mediam date string without time.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <returns></returns>
        public static string FormatDateInMediumDate(this DateTime date)
        {
            return date.ToString("dd-MMM-y");
        }
        /// <summary>
        /// Formats the decimal.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string FormatDecimal(this decimal value)
        {
            return value.ToString("C2");
        }
        /// <summary>
        /// Formats the decimal.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string FormatDecimal(this decimal? value)
        {
            if (value.HasValue)
                return value.Value.ToString("C2");
            else
                return "0.00";
        }

        public static string FormatAmount(this object value)
        {
            decimal d;
            if (Decimal.TryParse(Convert.ToString(value), out d))
                if (d >= 0)
                    return String.Format("{0:$###,###,##0.00}", d);
                else
                    return String.Format("({0:$###,###,##0.00})", Math.Abs(d));
            else
                return Convert.ToString(value);
        }

        public static DateTime GetDateTimeByTimeZone(this DateTime currentDateTime, string timeZone)
        {
            return TimeZoneInfo.ConvertTimeBySystemTimeZoneId(currentDateTime, timeZone);
        }
        public static Stream StringToStream(this string s)
        {
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);
            sw.Write(s);
            sw.Flush();
            ms.Seek(0, SeekOrigin.Begin);
            return ms;
        }
    }
}
