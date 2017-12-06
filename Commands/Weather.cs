using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Microsoft.Extensions.Configuration;

namespace dotbot.Commands
{
    [Group("weather")]
    public class Weather : ModuleBase<SocketCommandContext>
    {
        private IConfigurationRoot _config;
        private string OwmApiUrl = "http://api.openweathermap.org/data/2.5/weather";
        private string GeoNamesUrl = "http://api.geonames.org/timezoneJSON";

        public Weather(IConfigurationRoot config)
        {
            _config = config;
            OwmApiUrl += $"?APPID={_config["tokens:owm"]}&units=metric";
            GeoNamesUrl += $"?username={_config["tokens:geonames"]}";
        }


        [Command]
        [Summary("gets the weather for your location")]
        public async Task GetWeather()
        {
            // TODO: check if user has saved location and do API lookup
            await ReplyAsync("good weather");
        }


        [Command]
        [Summary("look up the weather at a mentioned user's saved location")]
        public async Task LookupWeatherForUser([Summary("user to check time for")] IUser user)
        {
            // TODO: check if they're in DB
            // TODO: convert to tz
            await ReplyAsync($"it's __ weather for {user.Mention}");
        }


        [Command]
        [Summary("look up the weather at a specified location")]
        public async Task LookupWeather([Remainder] [Summary("location")] string location)
        {
            // TODO: API lookup
            await ReplyAsync($"the weather in {location} is crap");
        }


        [Command("save")]
        [Summary("save a location for timezone and weather")]
        public async Task SaveUserLocation([Remainder] [Summary("location")] string location)
        {
            // TODO: lookup location detail
            // TODO: save to db
            await ReplyAsync($"you location has been saved as {location}");
        }

    }
}
