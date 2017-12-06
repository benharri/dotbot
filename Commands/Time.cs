using Discord;
using Discord.Commands;
using dotbot.Core;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

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
                    var dt = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, ul.TimeZone);
                    await ReplyAsync($"it's {dt:g} in {ul.City}");
                }
                else
                    await ReplyAsync($"it's {DateTime.Now:g} Eastern Time (where the bot is hosted)");
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
                    var tz = db.UserLocations.Find(user.Id).TimeZone;
                    var dt = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, tz);
                    await ReplyAsync($"the time for {user.Mention} is {dt:g}");
                }
                else
                {
                    await ReplyAsync($"{user.Mention} does not have a saved location");
                }
            }
        }


        [Command]
        [Summary("check the time in an arbitrary location")]
        public async Task LookupTime([Remainder] [Summary("location")] string location)
        {
            // TODO: api lookup
            await ReplyAsync($"it is __ o'clock in {location}");
        }

        [Command("save")]
        [Summary("save your location")]
        public async Task SaveUserLocation([Remainder] [Summary("the location")] string location)
        {
            using (var db = new DotbotDbContext())
            {
                if (db.UserLocations.Any(u => u.Id == Context.User.Id))
                { // update existing
                    var loc = db.UserLocations.Find(Context.User.Id);
                    // TODO: update location for user
                }
                else
                { // save new record

                    // TODO: lookup location and save
                    db.UserLocations.Add(new UserLocation
                    {
                        Id = Context.User.Id,
                        // TODO: City = jsonresult
                    });
                }
                db.SaveChanges();
                await ReplyAsync($"your location has been updated to `City`");
            }
        }

    }
}
