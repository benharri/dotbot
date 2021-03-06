﻿using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using dotbot.Core;
using System.Linq;
using dotbot.Commands;
using System.Collections.Generic;

namespace dotbot.Services
{
    public class CommandHandlerService
    {
        private DotbotDb _db;
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private readonly IConfigurationRoot _config;
        private readonly IServiceProvider _provider;
        private readonly HangmanService _hangman;
        private readonly TicTacToeService _tic;
        private Dictionary<ulong, Poll> _polls;

        public CommandHandlerService(
            DiscordSocketClient discord,
            CommandService commands,
            IConfigurationRoot config,
            IServiceProvider provider,
            DotbotDb db,
            PollService polls,
            HangmanService hangman,
            TicTacToeService tic
        )
        {
            _discord = discord;
            _commands = commands;
            _config = config;
            _provider = provider;
            _db = db;
            _polls = polls.currentPolls;
            _hangman = hangman;
            _tic = tic;

            _discord.MessageReceived += OnMessageReceivedAsync;
        }

        private async Task OnMessageReceivedAsync(SocketMessage s)
        {
            var msg = s as SocketUserMessage;
            if (msg == null) return;
            // Ignore self and other bots when checking commands
            if (msg.Author.Id == _discord.CurrentUser.Id || msg.Author.IsBot) return;

            var context = new SocketCommandContext(_discord, msg);

            int argPos = 0;     // Check if the message has a valid command prefix
            if (msg.HasStringPrefix(_config["prefix"], ref argPos) || msg.HasMentionPrefix(_discord.CurrentUser, ref argPos))
            {
                var result = await _commands.ExecuteAsync(context, argPos, _provider);     // Execute the command
                if (result.IsSuccess) return;

                if (!result.IsSuccess && result.ToString() != "UnknownCommand: Unknown command.")
                    await context.Channel.SendMessageAsync(result.ToString());

                if (msg.HasStringPrefix(_config["prefix"], ref argPos))
                { // check for other conditions
                    var key = msg.Content.Substring(_config["prefix"].Length).Split(' ').First();

                    if (_db.Defs.Any(d => d.Id == key))
                    { // get def
                        await context.Channel.SendMessageAsync($"**{key}**: {_db.Defs.Find(key).Def}");
                    }
                    else if (_db.Images.Any(i => i.Id == key))
                    { // get img
                        await context.Channel.TriggerTypingAsync();
                        await context.Message.DeleteAsync();
                        await context.Channel.SendFileAsync($"UploadedImages/{_db.Images.Find(key).FilePath}", $"{key} by {context.User.Mention}");
                    }
                    else if (UnicodeFonts.Fonts.ContainsKey(key))
                    { // convert font
                        await context.Channel.SendMessageAsync(UnicodeFonts.Fonts[key].Convert(msg.Content.Substring(msg.Content.IndexOf(" ") + 1)));
                    }
                }
            }
            else
            {
                // add poll options
                var id = context.Channel.Id;
                if (_polls.ContainsKey(id) && _polls[id].Owner == context.User && !_polls[id].IsOpen)
                {
                    _polls[id].Options.Add(new PollOption { Text = msg.Content });
                    await context.Channel.SendMessageAsync($"{msg.Content} added!");
                }

                // handle hangman guess
                await _hangman.HandleMove(msg, context);

                // handle tictactoe games
                await _tic.HandleMove(msg, context);

            }
        }
    }
}
