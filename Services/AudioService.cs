//////////////////////////////////////////////////////////////////////
// https://gist.github.com/Joe4evr/773d3ce6cc10dbea6924d59bbfa3c62a //
//////////////////////////////////////////////////////////////////////

using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;

public class AudioService
{
    private readonly ConcurrentDictionary<ulong, IAudioClient> ConnectedChannels = new ConcurrentDictionary<ulong, IAudioClient>();


    public async Task JoinAudio(IGuild guild, IVoiceChannel target)
    {
        if (ConnectedChannels.TryGetValue(guild.Id, out IAudioClient client))
        {
            return;
        }
        if (target.Guild.Id != guild.Id)
        {
            return;
        }

        var audioClient = await target.ConnectAsync();

        if (ConnectedChannels.TryAdd(guild.Id, audioClient))
        {
            // If you add a method to log happenings from this service,
            // you can uncomment these commented lines to make use of that.
            //await Log(LogSeverity.Info, $"Connected to voice on {guild.Name}.");
        }
    }


    public async Task LeaveAudio(IGuild guild)
    {
        if (ConnectedChannels.TryRemove(guild.Id, out IAudioClient client))
        {
            await client.StopAsync();
            //await Log(LogSeverity.Info, $"Disconnected from voice on {guild.Name}.");
        }
    }


    public async Task SendAudioAsync(IGuild guild, IMessageChannel channel, string path)
    {
        // Your task: Get a full path to the file if the value of 'path' is only a filename.
        if (!File.Exists(path))
        {
            await channel.SendMessageAsync("File does not exist.");
            return;
        }
        if (ConnectedChannels.TryGetValue(guild.Id, out IAudioClient client))
        {
            //await Log(LogSeverity.Debug, $"Starting playback of {path} in {guild.Name}");
            using (var output = CreateStream(path).StandardOutput.BaseStream)
            using (var stream = client.CreatePCMStream(AudioApplication.Music))
            {
                try { await output.CopyToAsync(stream); }
                finally { await stream.FlushAsync(); }
            }
        }
    }


    private Process CreateStream(string path)
    {
        return Process.Start(new ProcessStartInfo
        {
            FileName = "ytdl-bin/ffmpeg.exe",
            Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",
            UseShellExecute = false,
            RedirectStandardOutput = true
        });
    }
}
