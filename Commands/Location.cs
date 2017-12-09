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
        public DotbotDb db { get; set; }
        private string OwmUrl;
        private string GeoNamesUrl;

        public Location(IConfigurationRoot config)
        {
            OwmUrl = $"{config["endpoints:owm"]}?APPID={config["tokens:owm"]}&units=metric";
            GeoNamesUrl = $"{config["endpoints:geonames"]}?username={config["tokens:geonames"]}";
        }


        [Command("savelocation")]
        [Summary("save your location so benbot can look up your weather and timezone")]
        public async Task SaveUserLocation([Remainder] [Summary("location")] string location)
        {
            await Context.Channel.TriggerTypingAsync();
            var owm = Utils.GetJson<OwmApiResult>($"{OwmUrl}&q={HttpUtility.UrlEncode(location)}");
            var geo = Utils.GetJson<GeonamesApiResult>($"{GeoNamesUrl}&lat={owm.coord.lat}&lng={owm.coord.lon}");

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
