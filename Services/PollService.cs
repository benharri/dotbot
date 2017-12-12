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
        private readonly DiscordSocketClient ;
        public Dictionary<ulong, Poll> currentPolls;

        public PollService(DiscordSocketClient discord)
        {
            currentPolls = new Dictionary<ulong, Poll>();

            discord.ReactionAdded += OnReactionAddedAsync;
        }

        private Task OnReactionAddedAsync(Cacheable<IUserMessage, ulong> CachedMessage, ISocketMessageChannel Channel, SocketReaction Reaction)
        {
            if (currentPolls.ContainsKey(Channel.Id))
            {
                currentPolls[Channel.Id].Options.First(o => o.Message.Id == CachedMessage.Id).Votes++;
            }
            Console.WriteLine($"{Reaction.Emote} reaction added to {CachedMessage.Id} in {Channel.Name}");
            return Task.CompletedTask;
        }

    }



    public class Poll
    {
        public List<PollOption> Options { get; set; }
        public bool IsOpen { get; set; }
        public IUser Owner { get; set; }
        public PollOption Winner => Options.OrderBy(o => o.Votes).Last();
    }

    public class PollOption
    {
        public string Text { get; set; }
        public IUserMessage Message { get; set; }
        public int Votes { get; set; }
        public override string ToString() => Text;
    }
}
