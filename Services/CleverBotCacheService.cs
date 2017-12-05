using Discord;
using System.Collections.Generic;

namespace dotbot.Services
{
    public class CleverBotCacheService
    {
        public Dictionary<ulong, string> Cache { get; set; }

        public CleverBotCacheService()
        {
            Cache = new Dictionary<ulong, string>();
        }
    }
}
