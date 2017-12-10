using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotbot.Commands
{
    public class Misc : ModuleBase<SocketCommandContext>
    {
        private readonly Random _rand;

        public Misc(Random rand)
        {
            _rand = rand;
        }


        [Command("roll")]
        [Summary("rolls an n-sided die (defaults to 6-sided")]
        public async Task RollDie([Summary("[number of sides]")] int sides = 6)
        {
            await Context.Message.DeleteAsync();
            var embed = new EmbedBuilder()
            {
                Title = "Dice Roll :game_die:",
                Color = new Color(255, 0, 0),
                Description = $"{Context.User.Mention} rolled a {_rand.Next(1, sides)} (d{sides})"
            };
            await ReplyAsync("", false, embed);
        }


        [Command("rand")]
        [Summary("gets a random number")]
        public async Task RandomInt([Summary("max number")] int max = 100)
        {
            await Context.Message.DeleteAsync();
            await ReplyAsync($"{Context.User.Mention}, your random number (between 1 and {max}) is {_rand.Next(1, max)}");
        }

        [Command("rand")]
        [Summary("gets a random number between min and max")]
        public async Task RandomInt([Summary("min number")] int min, [Summary("max number")] int max)
        {
            await Context.Message.DeleteAsync();
            await ReplyAsync($"{Context.User.Mention}, your random number (between {min} and {max}) is {_rand.Next(min, max)}");
        }


        [Command("8ball")]
        [Summary("ask the mighty 8-ball to determine your fortunes")]
        public async Task Ask8Ball([Remainder] [Summary("your question for the magic 8ball")] string question)
        {
            await Context.Message.DeleteAsync();
            var responses = new List<string>
            {
                "It is certain",
                "It is decidedly so",
                "Without a doubt",
                "Yes definitely",
                "You may rely on it",
                "As I see it, yes",
                "Most likely",
                "Outlook good",
                "Yes",
                "Signs point to yes",
                "Reply hazy try again",
                "Ask again later",
                "Better not tell you now",
                "Cannot predict now",
                "Concentrate and ask again",
                "Don't count on it",
                "My reply is no",
                "My sources say no",
                "Outlook not so good",
                "Very doubtful",
            };
            await ReplyAsync($"{Context.User.Mention} asked: *{question}*\n\n**{responses.OrderBy(q => Guid.NewGuid()).First()}**");
        }


        [Command("avatar")]
        [Alias("pfp", "profilepic")]
        [Summary("displays a user's profile picture")]
        public async Task SendAvatar([Summary("user to get pfp for")] IUser mentionedUser = null)
        {
            var user = mentionedUser ?? Context.User;
            await ReplyAsync($"the avatar for {user.Mention} is at {user.GetAvatarUrl(size: 1024)}");
        }


        [Command("lenny")]
        [Summary("you should know what this does")]
        public async Task Lenny()
        {
            await ReplyAsync(@"( ͡° ͜ʖ ͡°)");
        }


        [Command("shrug")]
        [Alias("meh")]
        public async Task Shrug()
        {
            await ReplyAsync(@"¯\\\_(ツ)\_/¯");
        }

    }
}
