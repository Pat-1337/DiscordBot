using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Discord;
using Discord.Audio;

namespace Daddy.Modules.Audio
{
    public class AudioService
    {
        private readonly ConcurrentDictionary<ulong, IAudioClient> ConnectedChannels = new ConcurrentDictionary<ulong, IAudioClient>();

        public async Task JoinAudio(IGuild guild, IVoiceChannel target)
        {
            IAudioClient client;
            if (ConnectedChannels.TryGetValue(guild.Id, out client))
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
            IAudioClient client;
            if (ConnectedChannels.TryRemove(guild.Id, out client))
            {
                await client.StopAsync();
            }
        }

        #region File Player

        public async Task SendAudioAsync(IGuild guild, IMessageChannel channel, string path)
        {
            // Your task: Get a full path to the file if the value of 'path' is only a filename.
            if (!File.Exists(path))
            {
                await channel.SendMessageAsync("File does not exist.");
                return;
            }
            IAudioClient client;
            if (ConnectedChannels.TryGetValue(guild.Id, out client))
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
                FileName = "ffmpeg",
                Arguments = $"-hide_banner -loglevel panic -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1",//-i {path} -ac 2 -f s16le -ar 48000 pipe:1
                UseShellExecute = false,
                RedirectStandardOutput = true
            });
        }

#endregion

        #region Piping

        public async Task ytSendAudioAsync(IGuild guild, IMessageChannel channel, string url)
        {
            IAudioClient client;
            if (ConnectedChannels.TryGetValue(guild.Id, out client))
            {
                //await Log(LogSeverity.Debug, $"Starting playback of {path} in {guild.Name}");
                using (var output = CreateUrlStream(url).StandardOutput.BaseStream)
                using (var stream = client.CreatePCMStream(AudioApplication.Music, 128 * 1024))
                {
                    try { await output.CopyToAsync(stream); }
                    finally { await stream.FlushAsync().ConfigureAwait(false); }
                }
            }
        }

        private Process CreateUrlStream(string url)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = "youtube-dl",
                Arguments = $"-o - \"{url}\" | ffmpeg -hide_banner -i pipe:0 -ac 2 -f s16le -ar 48000 pipe:1",//-o - {url} | ffmpeg -i pipe:0 -ac 2 -f s16le -ar 48000 pipe:1
                UseShellExecute = false,
                RedirectStandardOutput = true,
                CreateNoWindow = true
            });
        }

        #endregion

        #region Youtube-dl -> ffmpeg

        public string GetvidUrl(string url)
        {
            return Process.Start(new ProcessStartInfo()
            {
                FileName = @"youtube-dl",
                Arguments = $" -x -g \"{url}\"",
                UseShellExecute = false,
                RedirectStandardOutput = true
            }).StandardOutput.ReadLine();
        }

        private Process CreateStreamM(string path)
        {
            return Process.Start(new ProcessStartInfo
            {
                FileName = @"ffmpeg",
                Arguments = $" -i \"{path}\" -ac 2 -f s16le -ar 48000 pipe:1 -aq 2",
                UseShellExecute = false,
                RedirectStandardOutput = true
            });
        }

        public async Task SendStreamAsync(IGuild guild, IMessageChannel channel, string path)
        {
            IAudioClient client;
            if (ConnectedChannels.TryGetValue(guild.Id, out client))
            {
                //await Log(LogSeverity.Debug, $"Starting playback of {path} in {guild.Name}");
                using (var output = CreateStreamM(path).StandardOutput.BaseStream)
                using (var stream = client.CreatePCMStream(AudioApplication.Music, 128 * 1024))
                {
                    try { await output.CopyToAsync(stream); }
                    finally { await stream.FlushAsync(); }
                }
            }
        }

        #endregion
    }
}
