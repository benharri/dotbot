using Discord;
using Discord.Commands;
using dotbot.Core;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Web;
using NodaTime;

namespace dotbot.Commands
{
    [Group("time")]
    public class Time : ModuleBase<SocketCommandContext>
    {
        private readonly IConfigurationRoot _config;
        private string OwmApiUrl = "http://api.openweathermap.org/data/2.5/weather";
        private string GeoNamesUrl = "http://api.geonames.org/timezoneJSON";

        public Time(IConfigurationRoot config)
        {
            _config = config;
            OwmApiUrl += $"?APPID={_config["tokens:owm"]}&units=metric";
            GeoNamesUrl += $"?username={_config["tokens:geonames"]}";
        }

        [Command]
        [Summary("get the time at your location (or bot's location if you don't have one saved)")]
        public async Task GetTime()
        {
            using (var db = new DotbotDbContext())
            {
                if (db.UserLocations.Any(u => u.Id == Context.User.Id))
                {
                    var ul = db.UserLocations.Find(Context.User.Id);
                    await ReplyAsync($"it's {GetTimeFromIanaId(ul.TimeZone):g} in {ul.City}");
                }
                else await ReplyAsync($"it's {DateTime.Now:g} Eastern Time (where the bot is hosted)\n\nyou can save your location/timezone with `{_config["prefix"]}savelocation <city>`");
            }
        }


        [Command]
        [Summary("get the time for a user (if they have a location saved)")]
        public async Task GetUserLocationTime([Summary("user to get time for")] IUser user)
        {
            using (var db = new DotbotDbContext())
            {
                if (db.UserLocations.Any(u => u.Id == user.Id))
                {
                    var ul = db.UserLocations.Find(user.Id);
                    await ReplyAsync($"the time for {user.Mention} in {ul.City} is {GetTimeFromIanaId(ul.TimeZone):g}");
                }
                else
                {
                    await ReplyAsync($"{user.Mention} does not have a saved location\nit's {DateTime.Now:g} Eastern Time (US)");
                }
            }
        }


        [Command]
        [Summary("check the time in an arbitrary location")]
        public async Task LookupTime([Remainder] [Summary("location")] string location)
        {
            await Context.Channel.TriggerTypingAsync();
            var owm = Utils.GetJson<Weather.OwmApiResult>($"{OwmApiUrl}&q={HttpUtility.UrlEncode(location)}");
            var geo = Utils.GetJson<GeonamesApiResult>($"{GeoNamesUrl}&lat={owm.coord.lat}&lng={owm.coord.lon}");
            await ReplyAsync($"it is {GetTimeFromIanaId(geo.timezoneId)} in {owm.name}");
        }


        public class GeonamesApiResult
        {
            public string timezoneId { get; set; }
        }

        public static DateTime GetTimeFromIanaId(string tzId) => SystemClock.Instance.GetCurrentInstant().InZone(DateTimeZoneProviders.Tzdb[tzId]).ToDateTimeUnspecified();
    }
}
