using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace dotbot.Services
{
    public class StartupService
    {
        private readonly DiscordSocketClient _discord;
        private readonly CommandService _commands;
        private readonly IConfigurationRoot _config;

        public StartupService(
            DiscordSocketClient discord,
            CommandService commands,
            IConfigurationRoot config)
        {
            _config = config;
            _discord = discord;
            _commands = commands;
        }

        public async Task StartAsync()
        {
            string token = _config["tokens:discord"];
            if (string.IsNullOrWhiteSpace(token))
                throw new Exception("Please enter your bot's token into the `_config.json` file found in the applications root directory.");

            await _discord.LoginAsync(TokenType.Bot, token);
            await _discord.StartAsync();

            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }
    }
}
