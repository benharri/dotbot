using Discord;
using Discord.Commands;
using dotbot.Core;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace dotbot.Commands
{
    [Group("time")]
    public class Time : ModuleBase<SocketCommandContext>
    {
        [Command]
        [Summary("get the time at the bot's location")]
        public async Task GetTime()
        {
            await ReplyAsync($"{DateTime.Now:g}");
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

        [Command("save")]
        [Summary("save your location")]
        public async Task SaveUserLocation([Remainder] [Summary("the location")] string location)
        {
            using (var db = new DotbotDbContext())
            {
                if (db.UserLocations.Any(u => u.Id == Context.User.Id))
                {
                    var loc = db.UserLocations.Find(Context.User.Id);
                    // TODO: update location for user
                }
                else
                {
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
