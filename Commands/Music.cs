//////////////////////////////////////////////////////////////////////
// https://gist.github.com/Joe4evr/773d3ce6cc10dbea6924d59bbfa3c62a //
//////////////////////////////////////////////////////////////////////

using Discord;
using Discord.Commands;
using System.Threading.Tasks;

public class Music : ModuleBase<ICommandContext>
{
    private readonly AudioService _service;

    public Music(AudioService service)
    {
        _service = service;
    }

    // You *MUST* mark these commands with 'RunMode.Async'
    // otherwise the bot will not respond until the Task times out.
    [Command("join", RunMode = RunMode.Async)]
    public async Task JoinCmd()
    {
        await _service.JoinAudio(Context.Guild, (Context.User as IVoiceState).VoiceChannel);
    }

    // Remember to add preconditions to your commands,
    // this is merely the minimal amount necessary.
    // Adding more commands of your own is also encouraged.
    [Command("leave", RunMode = RunMode.Async)]
    public async Task LeaveCmd()
    {
        await _service.LeaveAudio(Context.Guild);
    }

    [Command("play", RunMode = RunMode.Async)]
    public async Task PlayCmd([Remainder] string song)
    {
        await _service.SendAudioAsync(Context.Guild, Context.Channel, song);
    }
}
