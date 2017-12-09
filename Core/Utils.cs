using Newtonsoft.Json;
using NodaTime;
using System;
using System.Net;

namespace dotbot.Core
{
    public class Utils
    {
        public static T GetJson<T>(string url)
        {
            var json = (new WebClient { Proxy = null }).DownloadString(url);
            return JsonConvert.DeserializeObject<T>(json);
        }

        public static double ConvertCToF(double c) => ((9.0 / 5.0) * c) + 32;

        public static DateTime IanaIdToDateTime(string tzId) => SystemClock.Instance.GetCurrentInstant().InZone(DateTimeZoneProviders.Tzdb[tzId]).ToDateTimeUnspecified();

    }
}
