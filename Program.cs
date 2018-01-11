using Discord;
using Discord.Commands;
using Discord.WebSocket;
using dotbot.Core;
using dotbot.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace dotbot
{
    public class Program
    {
        private IConfigurationRoot _config;

        public static void Main(string[] args)
            => new Program().StartAsync().GetAwaiter().GetResult();

        public async Task StartAsync()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("_config.json");
            _config = builder.Build();

            var services = new ServiceCollection()
                .AddSingleton(new DiscordSocketClient(new DiscordSocketConfig
                {
                    LogLevel = LogSeverity.Verbose,
                    MessageCacheSize = 1000
                }))
                .AddSingleton(new CommandService(new CommandServiceConfig
                {
                    LogLevel = LogSeverity.Verbose,
                    DefaultRunMode = RunMode.Async
                }))
                .AddSingleton<CommandHandlerService>()
                .AddSingleton<LoggingService>()
                .AddSingleton<StartupService>()
                .AddSingleton<CleverBotCacheService>()
                .AddSingleton<DotbotDb>()
                .AddSingleton<PollService>()
                .AddSingleton<HangmanService>()
                .AddSingleton<TicTacToeService>()
                .AddSingleton<AudioService>()
                .AddSingleton<Random>()
                .AddSingleton(_config);

            var provider = services.BuildServiceProvider();

            provider.GetRequiredService<LoggingService>();
            await provider.GetRequiredService<StartupService>().StartAsync();
            provider.GetRequiredService<CommandHandlerService>();

            await Task.Delay(-1);
        }

    }
}
