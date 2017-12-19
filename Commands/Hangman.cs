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
            await ReplyAsync($"{_games[gameId]}");
        }
    }


    public class HangmanSession
    {
        internal string SecretWord;
        internal char[] SecretWordLetters => SecretWord.ToCharArray().Distinct().OrderBy(a => a).ToArray();
        internal char[] GuessedLetters;
        private int Guesses;
        public bool GameOver => SecretWordLetters.SequenceEqual(GuessedLetters.OrderBy(c => c)) && Guesses < Hangman.Gallows.Length;

        public HangmanSession(string secretWord)
        {
            SecretWord = secretWord;
            Guesses = 0;
        }
        

        public override string ToString()
        {
            var Incorrects = GuessedLetters.Except(SecretWordLetters);
            return $"```{Hangman.Gallows[Guesses]}\n{ShowSecretWord()}\n\nGuessed Letters: {string.Join(' ', GuessedLetters)}\nIncorrect Letters: {string.Join(' ', Incorrects)}```";
        }

        public string ShowSecretWord() => $"Word: {SecretWord.ToCharArray().Select<char, string>(c => $"{(GuessedLetters.Contains(c) ? c : '_')} ")}";
    }
}
