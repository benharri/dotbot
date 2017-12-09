using Newtonsoft.Json;
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

    }
}
