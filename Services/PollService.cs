using Discord;
using Discord.WebSocket;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotbot.Services
{
    public class PollService
    {
        private DiscordSocketClient _discord;
        public Dictionary<ulong, Poll> currentPolls;

        public PollService(DiscordSocketClient discord)
        {
            currentPolls = new Dictionary<ulong, Poll>();

            _discord = discord;
            _discord.ReactionAdded += OnReactionAddedAsync;
        }

        private Task OnReactionAddedAsync(Cacheable<IUserMessage, ulong> CachedMessage, ISocketMessageChannel Channel, SocketReaction Reaction)
        {
            if (currentPolls.ContainsKey(Channel.Id) && Reaction.UserId != _discord.CurrentUser.Id)
                currentPolls[Channel.Id].Options.First(o => o.MessageId == Reaction.MessageId).Votes++;
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
        public ulong MessageId { get; set; }
        public int Votes { get; set; }
        public override string ToString() => Text;
    }
}
