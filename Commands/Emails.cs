using Discord;
using Discord.Commands;
using dotbot.Core;
using Microsoft.Extensions.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace dotbot.Commands
{
    [Group("email")]
    public class Emails : ModuleBase<SocketCommandContext>
    {
        private readonly IConfigurationRoot _config;
        public Emails(IConfigurationRoot config)
        {
            _config = config;
        }


        [Command]
        public async Task SendEmail(
            [Summary("user to send to")] IUser recipient, 
            [Summary("message to send")] [Remainder] string message
        ) {
            using (var db = new DotbotDbContext())
            {
                if (!db.Emails.Any(e => e.Id == recipient.Id))
                {
                    await ReplyAsync($"{recipient.Mention} does not have a saved email");
                    return;
                }

                var status = await ReplyAsync($"sending message");

                var smtp = new SmtpClient("smtp.gmail.com")
                {
                    EnableSsl = true,
                    Port = 587,
                    Credentials = new NetworkCredential(_config["gmail_login"], _config["tokens:gmail"])
                };
                smtp.Send(
                    $"{Context.User.Username}-{Context.Guild.Name}@benbot.tilde.team", 
                    db.Emails.Find(recipient.Id).EmailAddress, 
                    $"benbot message from {Context.User.Username}",
                    message
                );

                await Context.Message.DeleteAsync();
                await status.ModifyAsync(m => m.Content = $"message sent to {recipient.Mention}!");
            }
        }


        [Command("save")]
        [Summary("saves an email address to your profile")]
        public async Task SaveEmail([Summary("email to save")] string email)
        {
            using (var db = new DotbotDbContext())
            {
                await Context.Message.DeleteAsync();
                db.Emails.Add(new Email
                {
                    Id = Context.User.Id,
                    EmailAddress = email
                });
                db.SaveChanges();
                await ReplyAsync("your email has been saved");
            }
        }

    }
}
