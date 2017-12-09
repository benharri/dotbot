using Discord.Commands;
using dotbot.Core;
using dotbot.Services;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace dotbot.Commands
{
    public class CleverBot : ModuleBase<SocketCommandContext>
    {
        public CleverBotCacheService _cacheService { get; set; }
        private string CleverBotApiUrl;

        public CleverBot(IConfigurationRoot config)
        {
            CleverBotApiUrl = $"{config["endpoints:cleverbot"]}?key={config["tokens:cleverbot"]}&input=";
        }

        class CleverBotResponse
        {
            public string cs { get; set; }
            public string output { get; set; }
        }


        [Command("cleverbot")]
        [Alias("bb")]
        [Summary("talk to the bot")]
        public async Task ChatWithCleverBot([Remainder] [Summary("what you want to say to benbot")] string message)
        {
            await Context.Channel.TriggerTypingAsync();
            var cache = _cacheService.Cache;
            var id = Context.Channel.Id;
            var response = Utils.GetJson<CleverBotResponse>($"{CleverBotApiUrl}{message}{(cache.ContainsKey(id) ? $"&cs={cache[id]}" : "")}");
            cache[id] = response.cs;
            await ReplyAsync(response.output);
        }

    }
}
