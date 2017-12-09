using Discord.Commands;
using dotbot.Core;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace dotbot.Commands
{
    public class Location : ModuleBase<SocketCommandContext>
    {
        private readonly IConfigurationRoot _config;
        private string OwmApiUrl = "http://api.openweathermap.org/data/2.5/weather";
        private string GeoNamesUrl = "http://api.geonames.org/timezoneJSON";

        public Location(IConfigurationRoot config)
        {
            _config = config;
            OwmApiUrl += $"?APPID={_config["tokens:owm"]}&units=metric";
            GeoNamesUrl += $"?username={_config["tokens:geonames"]}";
        }


        [Command("savelocation")]
        [Summary("save your location so benbot can look up your weather and timezone")]
        public async Task SaveUserLocation([Remainder] [Summary("location")] string location)
        {
            var owm = Utils.GetJson<Weather.OwmApiResult>($"{OwmApiUrl}&q={HttpUtility.UrlEncode(location)}");
            var geo = Utils.GetJson<Time.GeonamesApiResult>($"{GeoNamesUrl}&lat={owm.coord.lat}&lng={owm.coord.lon}");
            using (var db = new DotbotDbContext())
            {
                if (db.UserLocations.Any(u => u.Id == Context.User.Id))
                { // update existing city
                    var loc = db.UserLocations.Find(Context.User.Id);
                    loc.CityId = owm.id;
                    loc.Lat = owm.coord.lat;
                    loc.Lng = owm.coord.lon;
                    loc.City = owm.name;
                    loc.TimeZone = geo.timezoneId;
                }
                else
                { // save new location
                    db.UserLocations.Add(new UserLocation
                    {
                        Id = Context.User.Id,
                        CityId = owm.id,
                        Lat = owm.coord.lat,
                        Lng = owm.coord.lon,
                        City = owm.name,
                        TimeZone = geo.timezoneId,
                    });
                }
                db.SaveChanges();
                await ReplyAsync($"your location has been set to {owm.name}");
            }
        }
    }
}
