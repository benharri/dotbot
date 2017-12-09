using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using dotbot.Core;
using System.Linq;

namespace dotbot.Services
{
    public class CommandHandlerService
    {
        private DotbotDb _db;
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private readonly IConfigurationRoot _config;
        private readonly IServiceProvider _provider;

        public CommandHandlerService(
            DiscordSocketClient discord,
            CommandService commands,
            IConfigurationRoot config,
            IServiceProvider provider,
            DotbotDb db
        ) {
            _discord = discord;
            _commands = commands;
            _config = config;
            _provider = provider;
            _db = db;

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

                if (msg.HasStringPrefix(_config["prefix"], ref argPos))
                {
                    var key = msg.Content.Substring(_config["prefix"].Length);
                    if (_db.Defs.Any(d => d.Id == key))
                    {
                        await context.Channel.SendMessageAsync($"**{key}**: {_db.Defs.Find(key).Def}");
                        return;
                    }
                    if (_db.Images.Any(i => i.Id == key))
                    {
                        await context.Channel.TriggerTypingAsync();
                        await context.Message.DeleteAsync();
                        var img = _db.Images.Find(key);
                        await context.Channel.SendFileAsync($"UploadedImages/{img.FilePath}", $"{img.Id} by {context.User.Mention}");
                        return;
                    }
                }

                if (!result.IsSuccess && result.ToString() != "UnknownCommand: Unknown command.")
                    await context.Channel.SendMessageAsync(result.ToString());
            }
        }
    }
}
