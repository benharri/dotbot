using Discord.WebSocket;
using dotbot.Commands;
using System.Collections.Generic;

namespace dotbot.Services
{
    public class HangmanService
    {
        private DiscordSocketClient _discord;
        public Dictionary<ulong, HangmanSession> _activeGames;

        public HangmanService(DiscordSocketClient discord)
        {
            _discord = discord;
            _activeGames = new Dictionary<ulong, HangmanSession>();
        }
    }
}
