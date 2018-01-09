using Discord.WebSocket;
using dotbot.Commands;
using System.Collections.Generic;
using System;
using System.Linq;
using Discord.Commands;
using System.Threading.Tasks;

namespace dotbot.Services
{
    public class TicTacToeService
    {
        private DiscordSocketClient _discord;
        public Dictionary<ulong, TicTacToeSession> _activeGames;

        public TicTacToeService(DiscordSocketClient discord)
        {
            _discord = discord;
            _activeGames = new Dictionary<ulong, TicTacToeSession>();
        }

        internal async Task HandleMove(SocketUserMessage msg, SocketCommandContext context)
        {
            var id = context.Channel.Id;



            if (_activeGames.ContainsKey(id))
            {
                var game = _activeGames[id];
                if (!game.Active || game.Players[game.Turn] != msg.Author.Id) return;
                await msg.DeleteAsync();
                await game.LastMessage.DeleteAsync();
                await context.Channel.SendMessageAsync($"{game.DoMove(msg)}\n\n{game}");
                game.LastMessage = msg;
            }
        }

    }
}
