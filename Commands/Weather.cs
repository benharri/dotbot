using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Configuration;
using System;
using dotbot.Core;
using System.Linq;
using System.Web;

namespace dotbot.Commands
{
    [Group("weather")]
    public class Weather : ModuleBase<SocketCommandContext>
    {
        private IConfigurationRoot _config;
        private string OwmUrl;

        public Weather(IConfigurationRoot config)
        {
            _config = config;
            OwmUrl = $"{_config["endpoints:owm"]}?APPID={_config["tokens:owm"]}&units=metric";
        }


        [Command]
        [Summary("gets the weather for your location")]
        public async Task GetWeather()
        {
            using (var db = new DotbotDbContext())
            {
                if (db.UserLocations.Any(u => u.Id == Context.User.Id))
                {
                    var url = $"{OwmUrl}&id={db.UserLocations.Find(Context.User.Id).CityId}";
                    await ReplyAsync("", embed: WeatherEmbed(Utils.GetJson<OwmApiResult>(url)));
                }
                else await ReplyAsync($"you don't have a location saved. look one up with `{_config["prefix"]}weather <search term>` or save a location for yourself with `{_config["prefix"]}savelocation <city>`");
            }
        }


        [Command]
        [Summary("look up the weather at a mentioned user's saved location")]
        public async Task LookupWeatherForUser([Summary("user to check time for")] IUser user)
        {
            using (var db = new DotbotDbContext())
            {
                if (db.UserLocations.Any(u => u.Id == user.Id))
                {
                    var url = $"{OwmUrl}&id={db.UserLocations.Find(user.Id).CityId}";
                    await ReplyAsync("", embed: WeatherEmbed(Utils.GetJson<OwmApiResult>(url)));
                }
                else await ReplyAsync($"{user.Mention} doesn't have a location saved. look one up with `{_config["prefix"]}weather <search term>` or save a location for yourself with `{_config["prefix"]}savelocation <city>`");
            }
        }


        [Command]
        [Summary("look up the weather at a specified location")]
        public async Task LookupWeather([Remainder] [Summary("location")] string location)
        {
            var url = $"{OwmUrl}&q={HttpUtility.UrlEncode(location)}";
            await ReplyAsync("", embed: WeatherEmbed(Utils.GetJson<OwmApiResult>(url)));
        }


        private Embed WeatherEmbed(OwmApiResult result)
        {
            var embed = new EmbedBuilder
            {
                Title = $"Weather in {result.name}, {result.sys.country}",
                ThumbnailUrl = $"http://openweathermap.org/img/w/{result.weather[0].icon}.png",
            };
            embed.AddField("Current Temp", $"{result.main.temp}°C ({Utils.ConvertCToF(result.main.temp)}°F)");
            embed.AddInlineField("Low", $"{result.main.temp_min}°C ({Utils.ConvertCToF(result.main.temp_min)}°F)");
            embed.AddInlineField("High", $"{result.main.temp_max}°C ({Utils.ConvertCToF(result.main.temp_max)}°F)");
            embed.AddField("Current Conditions", result.weather[0].description);
            embed.AddInlineField("Atmospheric Pressure", $"{result.main.pressure} hPa");
            embed.AddInlineField("Humidity", $"{result.main.humidity}%");
            embed.AddInlineField("Wind", $"{result.wind.speed} meters/second, {result.wind.deg}°");
            var UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).ToLocalTime();
            embed.AddInlineField("Sunrise", $"{UnixEpoch.AddSeconds(result.sys.sunrise):t}");
            embed.AddInlineField("Sunset", $"{UnixEpoch.AddSeconds(result.sys.sunset):t}");
            return embed;
        }

    }

    public class OwmApiResult
    {
        public int id { get; set; }
        public string name { get; set; }
        public class Coord
        {
            public double lon { get; set; }
            public double lat { get; set; }
        }
        public Coord coord { get; set; }
        public class Weather
        {
            public int id { get; set; }
            public string main { get; set; }
            public string description { get; set; }
            public string icon { get; set; }
        }
        public Weather[] weather { get; set; }
        public class WeatherMain
        {
            public double temp { get; set; }
            public double pressure { get; set; }
            public double humidity { get; set; }
            public double temp_min { get; set; }
            public double temp_max { get; set; }
        }
        public WeatherMain main { get; set; }
        public class Wind
        {
            public double speed { get; set; }
            public int deg { get; set; }
        }
        public Wind wind { get; set; }
        public class Sys
        {
            public string country { get; set; }
            public double sunrise { get; set; }
            public double sunset { get; set; }
        }
        public Sys sys { get; set; }
    }

}
