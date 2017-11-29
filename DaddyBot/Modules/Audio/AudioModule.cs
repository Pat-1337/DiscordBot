using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.Audio;
using Discord.Commands;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Daddy.Modules.Audio
{
    public class AudioModule : ModuleBase
    {
        // Scroll down further for the AudioService.
        // Like, way down
        private readonly AudioService _service;

        // Remember to add an instance of the AudioService
        // to your IServiceCollection when you initialize your bot
        public AudioModule(AudioService service)
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
        public async Task PlayCmd(string song)
        {
            await _service.SendAudioAsync(Context.Guild, Context.Channel, song);
        }

        //[Command("play", RunMode = RunMode.Async)]
        public async Task play(string url)
        {
            await _service.SendAudioAsync(Context.Guild, Context.Channel, url);
        }

        [Command("yt", RunMode = RunMode.Async)]
        public async Task ytPlayCmd(string url)
        {
            await _service.ytSendAudioAsync(Context.Guild, Context.Channel, url);
        }

        [Command("stream", RunMode = RunMode.Async), RequireUserPermission(GuildPermission.ManageChannels)]
        public async Task Stream(string url)
        {
            try
            {
                var voicechannel = (Context.Message.Author as IGuildUser)?.VoiceChannel;
                await _service.JoinAudio(voicechannel.Guild, voicechannel);
                await _service.SendStreamAsync(voicechannel.Guild, Context.Channel, _service.GetvidUrl(url));
                await _service.LeaveAudio(voicechannel.Guild);
            }
            catch (Exception ex)
            {
                await Main.Daddy.log("Stream error", LogSeverity.Error, "AudioModule", ex);
            }

        }
    }
}
