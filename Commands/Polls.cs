using Discord;
using Discord.Commands;
using dotbot.Services;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace dotbot.Commands
{
    [Group("poll")]
    public class Polls : ModuleBase<SocketCommandContext>
    {
        private Dictionary<ulong, Poll> _polls;
        private IConfigurationRoot _config;

        public Polls(PollService polls, IConfigurationRoot config)
        {
            _polls = polls.currentPolls;
            _config = config;
        }

        [Command]
        [Priority(0)]
        [Summary("create a new poll")]
        public async Task CreatePoll([Remainder] [Summary("poll options")] string options = null)
        {
            var pollId = Context.Channel.Id;
            if (_polls.ContainsKey(pollId))
                await ReplyAsync($"respond with some more options or start the poll with `{_config["prefix"]}poll start`");
            else
            {
                _polls[pollId] = new Poll
                {
                    Owner = Context.User,
                    IsOpen = false,
                    Options = options == null ? new List<PollOption>() : options.Split(" ").Select(o => new PollOption{ Text = o }).ToList()
                };
                await ReplyAsync($"you started a new poll. respond with some options and then start the poll with `{_config["prefix"]}poll start`");
            }
        }


        [Command("start")]
        [Priority(1)]
        [Summary("starts a poll!")]
        public async Task StartPoll()
        {
            var pollId = Context.Channel.Id;
            if (_polls.ContainsKey(pollId) && Context.User == _polls[pollId].Owner && !_polls[pollId].IsOpen)
            {
                if (_polls[pollId].IsOpen)
                {
                    await ReplyAsync("poll already started");
                    return;
                }

                foreach (var opt in _polls[pollId].Options)
                {
                    var msg = await ReplyAsync($"{opt.Text}");
                    await msg.AddReactionAsync(new Emoji("👍"));
                    opt.MessageId = msg.Id;
                    await Task.Delay(300);
                }
                _polls[pollId].IsOpen = true;
            }
            else await ReplyAsync($"no poll ready to start");
        }


        [Command("stop")]
        [Alias("finish", "resolve")]
        [Priority(1)]
        [Summary("gets winner of poll")]
        public async Task StopPoll()
        {
            var pollId = Context.Channel.Id;
            if (_polls.ContainsKey(pollId) && Context.User == _polls[pollId].Owner && _polls[pollId].IsOpen)
            {
                _polls[pollId].IsOpen = false;
                await base.ReplyAsync($"the winner was **{_polls[pollId].Winner}**\nwith {_polls[pollId].Winner.Votes} votes");
                _polls.Remove(pollId);
            }
            else await ReplyAsync("you haven't started any polls");
        }

    }
}
