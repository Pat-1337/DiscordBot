using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using Discord.Commands;
using Daddy.Modules;
using Daddy.Database;
using Discord.WebSocket;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;

namespace Daddy.Modules
{
    public class Administration : ModuleBase
    {
        JSON jSon = new JSON();
        Random _ran = new Random();

        [Command("mute", RunMode = RunMode.Async), Priority(0), Summary("Mutes a person."), RequireBotPermission(GuildPermission.ManageRoles), RequireUserPermissionAttribute(GuildPermission.KickMembers)]
        public async Task Mute([Summary("User")]SocketGuildUser arg, [Summary("Time")]int minutes = 0, [Summary("Reason"), Remainder()]string reason = null)
        {
            if (BaseCommands.IsVip(Context.User as SocketUser) && !BaseCommands.IsVip(arg)) {
                if (jSon._permWR(Context, BaseCommands.Commands.Mute).Result) {
                    await Context.Message.DeleteAsync();
                    var sw = Stopwatch.StartNew();
                    await arg.AddRolesAsync(arg.Guild.Roles.Where(y => y.Name.ToLower().Equals("muted")));
                    EmbedBuilder builder = new EmbedBuilder()
                    {
                        Title = $"**{arg.Username} has been muted!**",
                        Description = $"{(string.IsNullOrEmpty(reason) ? string.Empty : $"**Reason: {reason}**")}{(minutes.Equals(0) ? string.Empty : $"\n**Minutes: {minutes}**")}",
                        Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255))),
                    }.WithCurrentTimestamp();
                    sw.Stop();
                    await ReplyAsync(string.Empty, false, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms | {Context.User}")).Build());
                    await BaseCommands.SendReasonEmbedToUserDM(arg, Context.Guild, reason, BaseCommands.Commands.Mute, minutes);
                    if (minutes <= 360 && minutes > 0) {
                        await TimerClass._tMute(Context.Guild, arg.Id, TimeSpan.FromMinutes(minutes), new CancellationTokenSource().Token);
                    }
                    else {
                        await TimerClass._tMute(Context.Guild, arg.Id, TimeSpan.FromMinutes(360), new CancellationTokenSource().Token);
                    }
                }
            }
        }

        [Command("unmute", RunMode = RunMode.Async), Summary("Unmutes a person."), RequireBotPermission(GuildPermission.ManageRoles), RequireUserPermissionAttribute(GuildPermission.KickMembers)]
        public async Task _unMute([Summary("User")]SocketGuildUser arg)
        {
            if (BaseCommands.IsVip(Context.User as SocketUser) && !BaseCommands.IsVip(arg)) {
                if (jSon._permWR(Context, BaseCommands.Commands.Mute).Result) {
                    await Context.Message.DeleteAsync();
                    var sw = Stopwatch.StartNew();
                    await arg.RemoveRolesAsync(arg.Guild.Roles.Where(y => y.Name.ToLower().Equals("muted")));
                    EmbedBuilder builder = new EmbedBuilder()
                    {
                        Title = $"**{arg.Username} has been unmuted!**",
                        Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255))),
                    }.WithCurrentTimestamp();
                    sw.Stop();
                    await ReplyAsync(string.Empty, false, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms | {Context.User}")).Build());
                }
            }
        }

        [Command("fmute", RunMode = RunMode.Async), Summary("Mutes a person."), RequireBotPermission(GuildPermission.ManageRoles), RequireUserPermissionAttribute(GuildPermission.KickMembers)]
        public async Task _fMute([Summary("User")]SocketGuildUser arg, [Summary("Reason"), Remainder()]string reason = null)
        {
            if (BaseCommands.IsVip(Context.User as SocketUser) && !BaseCommands.IsVip(arg)) {
                if (jSon._permWR(Context, BaseCommands.Commands.Mute).Result) {
                    await Context.Message.DeleteAsync();
                    var sw = Stopwatch.StartNew();
                    await arg.RemoveRolesAsync(arg.Roles.Where(x => x.Id != Context.Guild.EveryoneRole.Id && x.Position < x.Guild.CurrentUser.Hierarchy));
                    await arg.AddRolesAsync(arg.Guild.Roles.Where(y => y.Name.ToLower().Equals("muted")));
                    EmbedBuilder builder = new EmbedBuilder()
                    {
                        Title = $"**{arg.Username} has been muted!**",
                        Description = $"{(string.IsNullOrEmpty(reason) ? string.Empty : $"**Reason: {reason}**")}",
                        Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)))
                    }.WithCurrentTimestamp();
                    sw.Stop();
                    await ReplyAsync(string.Empty, false, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms | {Context.User}")).Build());
                    await BaseCommands.SendReasonEmbedToUserDM(arg, Context.Guild, reason, BaseCommands.Commands.Mute);
                }
            }
        }

        [Command("normie", RunMode = RunMode.Async), Summary("Normies a person."), RequireBotPermission(GuildPermission.ManageRoles)]
        public async Task Normie([Summary("User")]SocketGuildUser arg, [Summary("Reason"), Remainder()]string reason = null)
        {
            if ((BaseCommands.IsVip(Context.User as SocketUser) || (Context.User as SocketGuildUser).Roles.Where(x => x.Name == "Tormentor").Count() > 0) && !BaseCommands.IsVip(arg)) {
                if (jSon._permWR(Context, BaseCommands.Commands.Normie).Result) {
                    await Context.Message.DeleteAsync();
                    var sw = Stopwatch.StartNew();
                    await arg.RemoveRolesAsync(arg.Roles.Where(x => x.Id != Context.Guild.EveryoneRole.Id && x.Position < x.Guild.CurrentUser.Hierarchy));
                    await arg.AddRolesAsync(arg.Guild.Roles.Where(y => y.Name.ToLower().Equals("normie")));
                    EmbedBuilder builder = new EmbedBuilder()
                    {
                        Title = $"**{arg.Username} has been Normied!**",
                        Description = $"{(string.IsNullOrEmpty(reason) ? string.Empty : $"**Reason: {reason}**")}",
                        Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)))
                    }.WithCurrentTimestamp();
                    sw.Stop();
                    await ReplyAsync(string.Empty, false, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms | {Context.User}")).Build());
                    await BaseCommands.SendReasonEmbedToUserDM(arg, Context.Guild, reason, BaseCommands.Commands.Normie);
                }
            }
        }

        [Command("kick", RunMode = RunMode.Async), Summary("Kicks a person."), RequireBotPermission(GuildPermission.KickMembers), RequireUserPermissionAttribute(GuildPermission.KickMembers)]
        public async Task Kick([Summary("User")]SocketGuildUser arg, [Summary("Reason"), Remainder()]string reason = null)
        {
            if (BaseCommands.IsVip(Context.User as SocketUser) && !BaseCommands.IsVip(arg)) {
                if (jSon._permWR(Context, BaseCommands.Commands.Kick).Result) {
                    await Context.Message.DeleteAsync();
                    var sw = Stopwatch.StartNew();
                    BaseCommands.noSend.Add(arg.Id);
                    await BaseCommands.SendReasonEmbedToUserDM(arg, Context.Guild, reason, BaseCommands.Commands.Kick);
                    await arg.KickAsync(reason: reason);
                    EmbedBuilder builder = new EmbedBuilder()
                    {
                        Title = $"**{arg.Username} has been kicked!**",
                        Description = $"{(string.IsNullOrEmpty(reason) ? string.Empty : $"**Reason: {reason}**")}",
                        Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)))
                    }.WithCurrentTimestamp();
                    sw.Stop();
                    await ReplyAsync(string.Empty, false, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms | {Context.User}")).Build());
                }
            }
        }

        [Command("ban", RunMode = RunMode.Async), Summary("Bans a person."), RequireBotPermission(GuildPermission.BanMembers), RequireUserPermissionAttribute(GuildPermission.BanMembers)]
        public async Task Ban([Summary("User")]SocketGuildUser arg, [Summary("Reason"), Remainder()]string reason = null)
        {
            if (BaseCommands.IsVip(Context.User as SocketUser) && !BaseCommands.IsVip(arg)) {
                if (jSon._permWR(Context, BaseCommands.Commands.Ban).Result) {
                    await Context.Message.DeleteAsync();
                    var sw = Stopwatch.StartNew();
                    BaseCommands.noSend.Add(arg.Id);
                    await BaseCommands.SendReasonEmbedToUserDM(user: arg, guild: Context.Guild, reason: reason, r: BaseCommands.Commands.Ban);
                    await Context.Guild.AddBanAsync(user: arg, pruneDays: 1, reason: reason);
                    EmbedBuilder builder = new EmbedBuilder()
                    {
                        Title = $"**{arg.Username} has been banned!**",
                        Description = $"{(string.IsNullOrEmpty(reason) ? string.Empty : $"**Reason: {reason}**")}",
                        Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)))
                    }.WithCurrentTimestamp();
                    sw.Stop();
                    await ReplyAsync(string.Empty, false, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms | {Context.User}")).Build());
                }
            }
        }

        [Command("ban", RunMode = RunMode.Async), Summary("Bans a person."), RequireBotPermission(GuildPermission.BanMembers), RequireUserPermissionAttribute(GuildPermission.BanMembers)]
        public async Task Ban([Summary("ID of user")]ulong ID, [Summary("Reason"), Remainder()]string reason = null)
        {
            if (BaseCommands.IsVip(Context.User as SocketUser)) {
                if (jSon._permWR(Context, BaseCommands.Commands.Ban).Result) {
                    await Context.Message.DeleteAsync();
                    var sw = Stopwatch.StartNew();
                    BaseCommands.noSend.Add(ID);
                    await Context.Guild.AddBanAsync(userId: ID, pruneDays: 1, reason: reason);
                    EmbedBuilder builder = new EmbedBuilder()
                    {
                        Title = $"**{ID} has been banned!**",
                        Description = $"{(string.IsNullOrEmpty(reason) ? string.Empty : $"**Reason: {reason}**")}",
                        Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)))
                    }.WithCurrentTimestamp();
                    sw.Stop();
                    await ReplyAsync(string.Empty, false, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms | {Context.User}")).Build());
                }
            }
        }

        [Command("unban", RunMode = RunMode.Async), Summary("Unbans a person."), RequireBotPermission(GuildPermission.BanMembers), RequireUserPermissionAttribute(GuildPermission.BanMembers)]
        public async Task _unBan([Summary("ID of user")]ulong ID)
        {
            if (BaseCommands.IsVip(Context.User as SocketUser)) {
                if (jSon._permWR(Context, BaseCommands.Commands.Ban).Result) {
                    await Context.Message.DeleteAsync();
                    var sw = Stopwatch.StartNew();
                    await Context.Guild.RemoveBanAsync(userId: ID);
                    EmbedBuilder builder = new EmbedBuilder()
                    {
                        Title = $"**{ID} has been unbanned!**",
                        Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)))
                    }.WithCurrentTimestamp();
                    sw.Stop();
                    await ReplyAsync(string.Empty, false, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms | {Context.User}")).Build());
                }
            }
        }

        [Command("softban", RunMode = RunMode.Async), Summary("softBans a person."), RequireBotPermission(GuildPermission.BanMembers), RequireUserPermissionAttribute(GuildPermission.BanMembers)]
        public async Task Softban([Summary("User")]SocketGuildUser arg, [Summary("Reason"), Remainder()]string reason = null)
        {
            if (BaseCommands.IsVip(Context.User as SocketUser) && !BaseCommands.IsVip(arg)) {
                if (jSon._permWR(Context, BaseCommands.Commands.Softban).Result) {
                    var sw = Stopwatch.StartNew();
                    BaseCommands.noSend.Add(arg.Id);
                    await BaseCommands.SendReasonEmbedToUserDM(arg, Context.Guild, reason, BaseCommands.Commands.Softban);
                    await Context.Guild.AddBanAsync(arg, 5, reason: reason);
                    EmbedBuilder builder = new EmbedBuilder()
                    {
                        Title = $"**{arg.Username} has been softbanned!**",
                        Description = $"{(string.IsNullOrEmpty(reason) ? string.Empty : $"**Reason: {reason}**")}",
                        Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)))
                    }.WithCurrentTimestamp();
                    sw.Stop();
                    await ReplyAsync(string.Empty, false, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms | {Context.User}")).Build());
                    await Context.Guild.RemoveBanAsync(arg);
                }
            }
        }

        [Command("prune", RunMode = RunMode.Async), Priority(0), Summary("Prune messages"), RequireBotPermission(GuildPermission.ManageMessages), RequireUserPermissionAttribute(GuildPermission.ManageMessages)]
        public async Task Prune([Summary("User")]SocketGuildUser arg1 = null, [Summary("Number of msgs")]int arg2 = 100) => await Prune(arg2, arg1);

        [Command("prune", RunMode = RunMode.Async), Priority(1), Summary("Prune messages"), RequireBotPermission(GuildPermission.ManageMessages), RequireUserPermissionAttribute(GuildPermission.ManageMessages)]
        public async Task Prune([Summary("Number of msgs")]int arg1 = 100, [Summary("User")]SocketGuildUser arg2 = null)
        {
            if (BaseCommands.IsVip(Context.User as SocketUser)) {
                if (jSon._permWR(Context, BaseCommands.Commands.Prune).Result) {
                    if (arg1 > 250){
                        arg1 = 250;
                    }
                    var sw = Stopwatch.StartNew();
                    IEnumerable<IMessage> msgs = null;
                    if (arg2 == null) {
                        msgs = await Context.Channel.GetMessagesAsync(Context.Message, Direction.Before, arg1).FlattenAsync();
                    }
                    else {
                        msgs = (await Context.Channel.GetMessagesAsync(Context.Message, Direction.Before, 150).FlattenAsync()).Where(x => x.Author.Id == arg2.Id).Take(arg1);
                    }
                    await (Context.Channel as ITextChannel).DeleteMessagesAsync(msgs as IEnumerable<IMessage>);
                    sw.Stop();
                    await Context.Message.DeleteAsync();
                    EmbedBuilder builder = new EmbedBuilder()
                    {
                        Title = $"**{msgs.Count()} messages deleted!**",
                        Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)))
                    }.WithCurrentTimestamp();
                    await ReplyAsync(string.Empty, false, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms | {Context.User}")).Build());
                }
            }
        }

        [Command("iprune", RunMode = RunMode.Async), Priority(0), Summary("Prune messages"), RequireBotPermission(GuildPermission.ManageMessages), RequireUserPermissionAttribute(GuildPermission.ManageMessages)]
        public async Task _iPrune([Summary("User")]SocketGuildUser arg1 = null, [Summary("Number of msgs")]int arg2 = 10) => await _iPrune(arg2, arg1);

        [Command("iprune", RunMode = RunMode.Async), Priority(1), Summary("Prune messages"), RequireBotPermission(GuildPermission.ManageMessages), RequireUserPermissionAttribute(GuildPermission.ManageMessages)]
        public async Task _iPrune([Summary("Number of msgs")]int arg1 = 10, [Summary("User")]SocketGuildUser arg2 = null)
        {
            if (BaseCommands.IsVip(Context.User as SocketUser)) {
                if (jSon._permWR(Context, BaseCommands.Commands.Iprune).Result) {
                    if (arg1 > 100) {
                        arg1 = 100;
                    }
                    var sw = Stopwatch.StartNew();
                    IEnumerable<IMessage> msgs = null;
                    if (arg2 == null) {
                        msgs = (await Context.Channel.GetMessagesAsync(Context.Message, Direction.Before, 75).FlattenAsync()).Where(x => x.Attachments.Count != 0).Take(arg1);
                    }
                    else {
                        msgs = (await Context.Channel.GetMessagesAsync(Context.Message, Direction.Before, 100).FlattenAsync()).Where(x => x.Author.Id == arg2.Id && x.Attachments.Count != 0).Take(arg1);
                    }
                    await (Context.Channel as ITextChannel).DeleteMessagesAsync(msgs);
                    sw.Stop();
                    await Context.Message.DeleteAsync();
                    EmbedBuilder builder = new EmbedBuilder()
                    {
                        Title = $"**{msgs.Count()} messages deleted!**",
                        Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)))
                    }.WithCurrentTimestamp();
                    await ReplyAsync(string.Empty, false, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms | {Context.User}")).Build());
                }
            }
        }
    }

    public class TimerClass
    {
        public static async Task _tMute(IGuild guild, ulong ID, TimeSpan time, CancellationToken token)
        {
            await Task.Delay(time, token);
            IGuildUser x = await guild.GetUserAsync(ID);
            await x.RemoveRolesAsync(guild.Roles.Where(y => y.Name.ToLower().Equals("muted")));
        }
    }
}
