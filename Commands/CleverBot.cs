﻿using Discord.Commands;
using dotbot.Services;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace dotbot.Commands
{
    public class CleverBot : ModuleBase<SocketCommandContext>
    {
        private readonly IConfigurationRoot _config;
        private Dictionary<ulong, string> _cache;
        private string CleverBotApiUrl = "https://www.cleverbot.com/getreply";

        public CleverBot(IConfigurationRoot config, CleverBotCacheService cache)
        {
            _config = config;
            _cache = cache.Cache;
            CleverBotApiUrl += $"?key={_config["tokens:cleverbot"]}&input=";
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
            var url = $"{CleverBotApiUrl}{message}";
            if (_cache.ContainsKey(Context.Channel.Id))
                url += $"&cs={_cache[Context.Channel.Id]}";
            var json = (new WebClient { Proxy = null }).DownloadString(url);
            var response = JsonConvert.DeserializeObject<CleverBotResponse>(json);
            _cache[Context.Channel.Id] = response.cs;
            await ReplyAsync(response.output);
        }

    }
}
