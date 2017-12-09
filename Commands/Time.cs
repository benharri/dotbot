using Discord;
using Discord.Commands;
using dotbot.Core;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System.Web;

namespace dotbot.Commands
{
    [Group("time")]
    public class Time : ModuleBase<SocketCommandContext>
    {
        public DotbotDb db { get; set; }
        private readonly IConfigurationRoot _config;
        private string OwmUrl;
        private string GeoNamesUrl;

        public Time(IConfigurationRoot config)
        {
            _config = config;
            OwmUrl = $"{_config["endpoints:owm"]}?APPID={_config["tokens:owm"]}&units=metric";
            GeoNamesUrl = $"{_config["endpoints:geonames"]}?username={_config["tokens:geonames"]}";
        }


        [Command]
        [Summary("get the time at your location (or bot's location if you don't have one saved)")]
        public async Task GetTime()
        {
            await Context.Channel.TriggerTypingAsync();
            if (db.UserLocations.Any(u => u.Id == Context.User.Id))
            {
                var ul = db.UserLocations.Find(Context.User.Id);
                await ReplyAsync($"it's {Utils.IanaIdToDateTime(ul.TimeZone):g} in {ul.City}");
            }
            else await ReplyAsync($"it's {DateTime.Now:g} Eastern Time (where the bot is hosted)\n\nyou can save your location/timezone with `{_config["prefix"]}savelocation <city>`");
        }


        [Command]
        [Summary("get the time for a user (if they have a location saved)")]
        public async Task GetUserLocationTime([Summary("user to get time for")] IUser user)
        {
            await Context.Channel.TriggerTypingAsync();
            if (db.UserLocations.Any(u => u.Id == user.Id))
            {
                var ul = db.UserLocations.Find(user.Id);
                await ReplyAsync($"the time for {user.Mention} in {ul.City} is {Utils.IanaIdToDateTime(ul.TimeZone):g}");
            }
            else await ReplyAsync($"{user.Mention} does not have a saved location\nit's {DateTime.Now:g} Eastern Time (US)");
        }


        [Command]
        [Summary("check the time in an arbitrary location")]
        public async Task LookupTime([Remainder] [Summary("location")] string location)
        {
            await Context.Channel.TriggerTypingAsync();
            var owm = Utils.GetJson<OwmApiResult>($"{OwmUrl}&q={HttpUtility.UrlEncode(location)}");
            var geo = Utils.GetJson<GeonamesApiResult>($"{GeoNamesUrl}&lat={owm.coord.lat}&lng={owm.coord.lon}");
            await ReplyAsync($"it is {Utils.IanaIdToDateTime(geo.timezoneId)} in {owm.name}");
        }

    }


    public class GeonamesApiResult
    {
        public string timezoneId { get; set; }
    }

}
