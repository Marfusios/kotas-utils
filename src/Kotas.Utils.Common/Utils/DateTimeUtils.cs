using System;

namespace Kotas.Utils.Common.Utils
{
    public static class DateTimeUtils
    {
        public static readonly DateTime UnixBase = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public static TimeSpan UnixUtcNow()
        {
            var substracted = DateTime.UtcNow.Subtract(UnixBase);
            return substracted;
        }

        public static DateTime ConvertToUnixTime(long timeInMs)
        {
            var unix = UnixBase;
            return unix.AddMilliseconds(timeInMs);
        }
    }
}
