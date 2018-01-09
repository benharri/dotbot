using Discord.WebSocket;
using dotbot.Commands;
using System.Collections.Generic;
using System;
using Discord.Commands;
using System.Threading.Tasks;

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

        internal async Task HandleMove(SocketUserMessage msg, SocketCommandContext context)
        {
            var id = context.Channel.Id;

            if (_activeGames.ContainsKey(id))
            {
                var game = _activeGames[id];
                if (msg.Content.Length != 1)
                {
                    await context.Channel.SendMessageAsync($"send one letter at a time.\n\n{game}");
                    return;
                }

                if (game.Guess(msg.Content[0]))
                { // proper guess
                    if (game.GameOver)
                    {
                        await context.Channel.SendMessageAsync($"game over!\n\n{game}");
                        return;
                    }
                    await context.Channel.SendMessageAsync($"{msg.Content[0]} guessed:\n\n{game}");
                }
                else
                {
                    await context.Channel.SendMessageAsync($"letter already guessed... try again!\n\n{game}");
                }
            }
        }

    }
}
