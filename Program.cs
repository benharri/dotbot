using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace dotbot
{
    public class Program
    {
        private CommandService _commands;
        private DiscordSocketClient _client;
        private IServiceProvider _services;

        public static void Main(string[] args)
            => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            _client = new DiscordSocketClient();
            _commands = new CommandService();

            _client.Log += Log;

            DotNetEnv.Env.Load();
            string token = Environment.GetEnvironmentVariable("TOKEN");

            _services = new ServiceCollection()
                .AddSingleton(_client)
                .AddSingleton(_commands)
                .BuildServiceProvider();

            await InstallCommandsAsync();

            await _client.LoginAsync(TokenType.Bot, token);
            await _client.StartAsync();

            await Task.Delay(-1);
        }

        public async Task InstallCommandsAsync()
        {
            _client.MessageReceived += HandleCommandAsync;
            await _commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private async Task HandleCommandAsync(SocketMessage arg)
        {
            var message = arg as SocketUserMessage;
            if (message == null) return;

            int argPos = 0;

            if (!(message.HasCharPrefix(';', ref argPos) || message.HasMentionPrefix(_client.CurrentUser, ref argPos))) return;

            var context = new SocketCommandContext(_client, message);

            var result = await _commands.ExecuteAsync(context, argPos, _services);

            if (!result.IsSuccess)
                await context.Channel.SendMessageAsync(result.ErrorReason);

        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
