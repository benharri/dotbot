using Discord.WebSocket;

namespace dotbot.Services
{
    public class HangmanService
    {
        private DiscordSocketClient _discord;
        private Dictionary<ulong, HangmanSession> _activeGames;

        public HangmanService(DiscordSocketClient discord)
        {
            _discord = discord;
        }
    }
}
