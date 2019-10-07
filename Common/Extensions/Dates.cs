using System;

namespace Common.Extensions
{
    public static class Dates
    {
        public static string ToIsoDateStr(this DateTime date)
        {
            return date.ToString("yyyy-MM-dd");
        }

        public static string ToIsoTimeStr(this DateTime date)
        {
            return date.ToString("HH:mm:ss");
        }

        public static string ToIsoDateTimeStr(this DateTime date)
        {
            return date.ToString("yyyy-MM-ddTHH:mm:sszzz");
        }

        public static string ToIsoDateTimeStrWithoutTimeZone(this DateTime date)
        {
            return date.ToString("yyyy-MM-ddTHH:mm:ss.fff");
        }

        public static long ToUnixTime(this DateTime date)
        {
            return ((date.ToUniversalTime().Ticks - 621355968000000000)/10000000);
        }
    }
}
