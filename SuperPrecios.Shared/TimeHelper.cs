using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SuperPrecios.Shared
{
    public static class TimeHelper
    {
        private static readonly TimeZoneInfo MontevideoTimeZone =
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? TimeZoneInfo.FindSystemTimeZoneById("Montevideo Standard Time")
                : TimeZoneInfo.FindSystemTimeZoneById("America/Montevideo");

        public static DateTime NowInMontevideo()
        {
            return TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, MontevideoTimeZone);
        }

        public static DateOnly DateOnlyNowInMontevideo()
        {
            return DateOnly.FromDateTime(NowInMontevideo());
        }

        public static TimeOnly TimeOnlyNowInMontevideo()
        {
            return TimeOnly.FromDateTime(NowInMontevideo());
        }
    }
}
