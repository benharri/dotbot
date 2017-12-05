using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Threading.Tasks;
using Discord;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace dotbot.Commands
{
    public class Status : ModuleBase<SocketCommandContext>
    {
        [Command("userinfo")]
        [Summary("Returns info about the current user, or the user parameter, if one passed.")]
        [Alias("user", "whois", "whoami")]
        public async Task UserInfoAsync([Summary("The (optional) user to get info for")] SocketGuildUser infoUser = null)
        {
            if (Context.IsPrivate)
            {
                await ReplyAsync("you're not in a server. you already know who you are.");
                return;
            }

            var user = infoUser ?? Context.Guild.CurrentUser;
            var embed = new EmbedBuilder
            {
                Title = "User Info",
                Description = $"{user.Mention}",
                ThumbnailUrl = user.GetAvatarUrl(),
                Color = new Color(0, 0, 255),
            };
            embed.AddField("ID", $"{user.Id}");
            embed.AddField("Status", $"{user.Status}");
            embed.AddField("Game", $"{user.Game?.ToString() ?? "not playing anything right now"}");
            embed.AddField("Joined At", $"{user.JoinedAt:f}");

            await ReplyAsync("", false, embed);
        }


        [Command("status")]
        [Summary("gets info about the bot")]
        public async Task BotStatus()
        {
            var guilds = Context.Client.Guilds;
            var uptime = DateTime.UtcNow - Process.GetCurrentProcess().StartTime.ToUniversalTime();
            var embed = new EmbedBuilder
            {
                Title = $"About {Context.Client.CurrentUser.Username}",
                ThumbnailUrl = Context.Client.CurrentUser.GetAvatarUrl(),
                Description = "Here are some of my stats",
                Color = new Color(0, 255, 0),
            };
            embed.AddField("Server Count", $"{guilds.Count}");
            embed.AddField("User Count", $"{guilds.Sum(g => g.Users.Count)}");
            embed.AddField("Bot Uptime", $"{uptime:g}");
            embed.AddField("Bot Memory Usage", $"{GC.GetTotalMemory(false) / 1000} mb");

            await ReplyAsync("", false, embed);
        }


        [Command("server")]
        [Alias("guild", "serverinfo", "guildinfo")]
        [Summary("gets info about the current server/guild")]
        public async Task GuildInfo()
        {
            if (Context.IsPrivate)
            {
                await ReplyAsync("you're not in a server, silly");
                return;
            }

            var embed = new EmbedBuilder
            {
                Title = $"Server Info for: {Context.Guild.Name}",
                ThumbnailUrl = Context.Guild.IconUrl,
                Description = "Here is some info about this server",
                Color = new Color(0, 0, 255),
            };
            var guild = Context.Guild;
            embed.AddInlineField("Server Owner", $"{guild.Owner.Mention}");
            embed.AddInlineField("Region", $"{guild.VoiceRegionId}");
            embed.AddInlineField("User Count", $"{guild.MemberCount}");
            embed.AddInlineField("Channel Count", $"{guild.Channels.Count}");
            embed.AddField("Created At", $"{guild.CreatedAt:f}");
            embed.AddField("Verification Level", $"{guild.VerificationLevel}");
            embed.AddField(
                $"{Context.Client.CurrentUser.Username} Joined At",
                $"{Context.Guild.GetUser(Context.Client.CurrentUser.Id).JoinedAt:f}"
            );
            embed.AddField("Server ID", $"{guild.Id}");

            await ReplyAsync("", false, embed);
        }

    }
}
