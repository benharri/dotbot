using Discord.Commands;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace dotbot.Commands
{
    public class AsciiArt : ModuleBase<SocketCommandContext>
    {
        private readonly IConfigurationRoot _config;

        public AsciiArt(IConfigurationRoot config) {
            _config = config;
        }

        [Command("ascii")]
        [Summary("creates ascii word art")]
        public async Task CreateAsciiArt(
            [Summary("font you want to use")] string fontName,
            [Remainder] [Summary("text to convert")] string message = ""
        ) {
            if (fontName == "list")
            {
                var fontList = Directory.GetFiles("Fonts").ToList().Select(Path.GetFileNameWithoutExtension);
                var joinedList = string.Join(", ", fontList);
                Console.WriteLine(joinedList);
                await ReplyAsync($"available fonts for use with `{_config["prefix"]}ascii`:\n```{joinedList}```");
            }
            else if (File.Exists($"Fonts/{fontName}.flf"))
            {
                using (FileStream fs = File.OpenRead($"Fonts/{fontName}.flf")) {
                    var font = new WenceyWang.FIGlet.FIGletFont(fs);
                    await ReplyAsync($"```\n{(new WenceyWang.FIGlet.AsciiArt(message, font: font)).ToString()}\n```");
                }
            }
            else
            {
                await ReplyAsync($"```\n{(new WenceyWang.FIGlet.AsciiArt(fontName + message)).ToString()}\n```");
            }
        }
    }
}

