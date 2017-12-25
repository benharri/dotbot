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
            var gameId = Context.Channel.Id;
            _games.Add(gameId, new HangmanSession(secret));
            await Context.Message.DeleteAsync();
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
                await ReplyAsync($"game stopped. the secret word was {game.SecretWord}");
            }
            else
            {
                await ReplyAsync($"no game started in this channel...");
            }
        }
    }


    public class HangmanSession
    {
        internal string SecretWord;
        internal IEnumerable<char> SecretWordLetters => SecretWord.ToCharArray().Distinct().OrderBy(a => a);
        internal List<char> GuessedLetters;
        private int Guesses;
        public bool GameOver => SecretWordLetters.SequenceEqual(GuessedLetters.OrderBy(c => c)) && Guesses >= Hangman.Gallows.Length;

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
        {
            var Incorrects = GuessedLetters.Except(SecretWordLetters);
            return $"```{Hangman.Gallows[Guesses]}\n{ShowSecretWord()}\n\nGuessed Letters: {string.Join(' ', GuessedLetters)}\nIncorrect Letters: {string.Join(' ', Incorrects)}```";
        }

        public string ShowSecretWord()
        {
            var disp = SecretWord.ToCharArray().Select(c => $"{(GuessedLetters.Contains(c) ? c : '_')} ");
            return $"Word: {string.Join("", disp)}";
        }
    }
}
