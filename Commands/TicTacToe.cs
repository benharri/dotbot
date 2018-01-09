using Discord.Commands;
using dotbot.Services;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;

namespace dotbot.Commands
{
    [Group("tic")]
    public class TicTacToe : ModuleBase<SocketCommandContext>
    {
        public Dictionary<ulong, TicTacToeSession> _games;


        public TicTacToe(TicTacToeService tic)
        {
            _games = tic._activeGames;
        }

        [Command]
        [Priority(0)]
        [Summary("start a game of tic tac toe!")]
        public async Task StartGame([Summary("mention whom you would like to play with!")] IUser opponent)
        {
            var gameId = Context.Channel.Id;
            _games.Add(gameId, new TicTacToeSession(opponent));
            await ReplyAsync($"{_games[gameId]}");
        }


        [Command("stop")]
        [Priority(1)]
        [Summary("ends a tic tac toe session")]
        public async Task StopGame()
        {
            var gameId = Context.Channel.Id;
            if (_games.ContainsKey(gameId))
            {
                var game = _games[gameId];
                _games.Remove(gameId);
                await ReplyAsync("game over.");
            }
            else
            {
                await ReplyAsync("no game running in this channel...");
            }
        }
    }


    public class TicTacToeSession
    {

        public TicTacToeSession(IUser opponent)
        {

        }


        public override string ToString()
            => $"tictactoe";

    }
}
