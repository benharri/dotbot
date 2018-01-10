using Discord.Commands;
using dotbot.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using System;
using Discord.WebSocket;

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
            _games[gameId] = new TicTacToeSession(Context.User, opponent) { LastMessage = Context.Message };
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
        public bool Active;
        public bool Tied;
        public Dictionary<string, ulong> Players;
        public string Turn;
        public string[][] Board;
        public IMessage LastMessage;

        public TicTacToeSession(IUser player, IUser opponent)
        {
            Board = new string[][] {
                new string[] { ":one:", ":two:", ":three:" },
                new string[] { ":four:", ":five:", ":six:" },
                new string[] { ":seven:", ":eight:", ":nine:" }
            };
            Players = new Dictionary<string, ulong>
            {
                { ":x:", player.Id },
                { ":o:", opponent.Id }
            };
            Turn = ":x:";
            Active = true;
            Tied = false;
        }


        public string GetPiece(int i) => Board[(i - 1) / 3][(i - 1) % 3];

        public bool PutPiece(int i, string piece)
        {
            if (GetPiece(i) == ":x:" || GetPiece(i) == ":o:") return false;
            Board[(i - 1) / 3][(i - 1) % 3] = piece;
            return true;
        }

        internal string DoMove(SocketUserMessage msg)
        {
            if (Int32.TryParse(msg.Content, out var move))
                if (move > 0 && move < 10)
                    if (!PutPiece(move, Turn))
                        return $"unable to place your piece. position **{move}** already occupied by {GetPiece(move)}";
                    else
                    {
                        if (CheckWin())
                        {
                            Active = false;
                            return $"<@{Players[Turn]}> ({Turn}) won!";
                        }
                        else if (Tied)
                        {
                            Active = false;
                            return "it's a tie. game over.";
                        }
                        else
                        {
                            Turn = Turn == ":x:" ? ":o:" : ":x:";
                            return $"";
                        }
                    }
                else return $"**{msg.Content}** is not a valid move. please enter a number between 1 and 9.";
            else return "your move wasn't even a number... try again!";
        }


        internal bool CheckWin()
        {
            if ((GetPiece(1) == GetPiece(4) && GetPiece(4) == GetPiece(7))
             || (GetPiece(2) == GetPiece(5) && GetPiece(5) == GetPiece(8))
             || (GetPiece(3) == GetPiece(6) && GetPiece(6) == GetPiece(9))
             || (GetPiece(1) == GetPiece(2) && GetPiece(2) == GetPiece(3))
             || (GetPiece(4) == GetPiece(5) && GetPiece(5) == GetPiece(6))
             || (GetPiece(7) == GetPiece(8) && GetPiece(8) == GetPiece(9))
             || (GetPiece(1) == GetPiece(5) && GetPiece(5) == GetPiece(9))
             || (GetPiece(3) == GetPiece(5) && GetPiece(5) == GetPiece(7))
            ) return true;
            else
            {
                if (Enumerable.Range(1, 9).All(i => GetPiece(i) == ":o:" || GetPiece(i) == ":x:"))
                {   // check tie condition (all pieces placed)
                    Tied = true;
                    Active = false;
                }
                return false;
            }
        }

        public override string ToString() => $"{string.Join("\n", Board.Select(r => $"{string.Join(" ", r)}"))}\n<@{Players[Turn]}>'s turn. send a number 1-9.";

    }
}
