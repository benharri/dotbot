using Discord.Commands;
using dotbot.Services;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace dotbot.Commands
{
    [Group("hangman")]
    public class Hangman : ModuleBase<SocketCommandContext>
    {
        static internal string[] Gallows = File.ReadAllText("gallows.txt").Split('=');
        public Dictionary<ulong, HangmanSession> _games;


        public Hangman(HangmanService hangman)
        {
            _games = hangman._activeGames;
        }

        [Command]
        [Priority(0)]
        [Summary("start a game of hangman!")]
        public async Task StartGame([Remainder] string secret)
        {
            await Context.Message.DeleteAsync();
            var gameId = Context.Channel.Id;
            _games.Add(gameId, new HangmanSession(secret));
            await ReplyAsync($"{_games[gameId]}");
        }


        [Command("stop")]
        [Priority(1)]
        [Summary("ends a hangman session")]
        public async Task StopGame()
        {
            var gameId = Context.Channel.Id;
            if (_games.ContainsKey(gameId))
            {
                var game = _games[gameId];
                _games.Remove(gameId);
                await ReplyAsync($"game over. the secret word was {game.SecretWord}");
            }
            else
            {
                await ReplyAsync("no game running in this channel...");
            }
        }
    }


    public class HangmanSession
    {
        internal string SecretWord;
        internal IEnumerable<char> SecretWordLetters
            => SecretWord.ToCharArray().Distinct().OrderBy(a => a);
        public string ShowSecretWord
            => $"Word: {string.Join("", SecretWord.ToCharArray().Select(c => $"{(GuessedLetters.Contains(c) ? c : c == ' ' ? ' ' : '_')} "))}";
        internal List<char> GuessedLetters;
        private int Guesses;
        public bool GameOver
            => SecretWordLetters.SequenceEqual(GuessedLetters.OrderBy(c => c))
            || Guesses >= Hangman.Gallows.Length;

        public HangmanSession(string secretWord)
        {
            SecretWord = secretWord;
            GuessedLetters = new List<char>();
            Guesses = 0;
        }


        public bool Guess(char guess)
        {
            if (GuessedLetters.Contains(guess)) return false;
            GuessedLetters.Add(guess);
            if (!SecretWordLetters.Contains(guess)) Guesses++;
            return true;
        }


        public override string ToString()
            => $"```{Hangman.Gallows[Guesses]}\n{ShowSecretWord}\n\n"
             + $"Guessed Letters: {string.Join(' ', GuessedLetters)}\n"
             + $"Incorrect Letters: {string.Join(' ', GuessedLetters.Except(SecretWordLetters))}```";

    }
}
