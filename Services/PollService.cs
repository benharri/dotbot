using Discord;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace dotbot.Services
{
    public class PollService
    {
        private readonly DiscordSocketClient _discord;
        public Dictionary<ulong, Poll> currentPolls;

        public PollService(DiscordSocketClient discord)
        {
            currentPolls = new Dictionary<ulong, Poll>();
            _discord = discord;

            _discord.ReactionAdded += OnReactionAddedAsync;
        }

        private Task OnReactionAddedAsync(Cacheable<IUserMessage, ulong> CachedMessage, ISocketMessageChannel Channel, SocketReaction Reaction)
        {
            if (currentPolls.ContainsKey(Channel.Id))
            {
                currentPolls[Channel.Id].Options.First(o => o.Message.Id == CachedMessage.Id).Message = CachedMessage.Value;
            }
            Console.WriteLine(CachedMessage.Value.Reactions.Count);

            return Task.CompletedTask;
        }

        //var msg = s as SocketUserMessage;
        //if (msg == null) return;
        //var context = new SocketCommandContext(_discord, msg);
        //if (!currentPolls.ContainsKey(context.Channel.Id)) return;


    }



    public class Poll
    {
        public List<PollOption> Options { get; set; }
        public bool IsOpen { get; set; }
        public IUser Owner { get; set; }
        public PollOption Winner => Options.OrderBy(o => o.Votes).First();
    }

    public class PollOption
    {
        public string Text { get; set; }
        public IUserMessage Message { get; set; }
        public int Votes => Message.Reactions.Count;
        public override string ToString() => Text;
    }
}
