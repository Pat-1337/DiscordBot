﻿using System;
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

namespace Daddy.Modules
{
    public class Administation : ModuleBase
    {
        JSON jSon = new JSON();
        Random _ran = new Random();

        [Command("mute", RunMode = RunMode.Async), Summary("Mutes a person."), RequireBotPermission(GuildPermission.KickMembers), RequireUserPermissionAttribute(GuildPermission.KickMembers)]
        public async Task Mute([Summary("User")]SocketGuildUser arg, [Summary("Reason"), Remainder()]string reason = null)
        {
            if (BaseCommands.isVip(Context.User as SocketUser))
            {
                if (jSon.checkPermChn(Context.Guild, Context.Channel.Id, BaseCommands.Commands.Mute))
                {
                    await Context.Message.DeleteAsync();
                    var sw = Stopwatch.StartNew();
                    await arg.AddRolesAsync(arg.Guild.Roles.Where(y => y.Name.ToLower().Equals("muted")));
                    EmbedBuilder builder = new EmbedBuilder()
                    {
                        Title = $"**{arg.Username} has been muted!**",
                        Description = $"{(string.IsNullOrEmpty(reason) ? "" : $"**Reason: {reason}**")}",
                        Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255))),
                    }.WithCurrentTimestamp();
                    sw.Stop();
                    await ReplyAsync(string.Empty, false, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
                    await BaseCommands.sendReasonEmbedToUserDM(arg, Context.Guild, reason, BaseCommands.Commands.Mute);
                }
                else
                {
                    await ReplyAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
                }
            }
        }

        [Command("unmute", RunMode = RunMode.Async), Summary("Unmutes a person."), RequireBotPermission(GuildPermission.KickMembers), RequireUserPermissionAttribute(GuildPermission.KickMembers)]
        public async Task unMute([Summary("User")]SocketGuildUser arg)
        {
            if (BaseCommands.isVip(Context.User as SocketUser))
            {
                if (jSon.checkPermChn(Context.Guild, Context.Channel.Id, BaseCommands.Commands.Mute))
                {
                    await Context.Message.DeleteAsync();
                    var sw = Stopwatch.StartNew();
                    await arg.RemoveRolesAsync(arg.Guild.Roles.Where(y => y.Name.ToLower().Equals("muted")));
                    EmbedBuilder builder = new EmbedBuilder()
                    {
                        Title = $"**{arg.Username} has been unmuted!**",
                        Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255))),
                    }.WithCurrentTimestamp();
                    sw.Stop();
                    await ReplyAsync(string.Empty, false, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
                }
                else
                {
                    await ReplyAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
                }
            }
        }

        [Command("fmute", RunMode = RunMode.Async), Summary("Mutes a person."), RequireBotPermission(GuildPermission.KickMembers), RequireUserPermissionAttribute(GuildPermission.KickMembers)]
        public async Task fMute([Summary("User")]SocketGuildUser arg, [Summary("Reason"), Remainder()]string reason = null)
        {
            if (BaseCommands.isVip(Context.User as SocketUser))
            {
                if (jSon.checkPermChn(Context.Guild, Context.Channel.Id, BaseCommands.Commands.Mute))
                {
                    await Context.Message.DeleteAsync();
                    var sw = Stopwatch.StartNew();
                    await arg.RemoveRolesAsync(arg.Roles.Where(x => x.Id != Context.Guild.EveryoneRole.Id && x.Position < x.Guild.CurrentUser.Hierarchy));
                    await arg.AddRolesAsync(arg.Guild.Roles.Where(y => y.Name.ToLower().Equals("muted")));
                    EmbedBuilder builder = new EmbedBuilder()
                    {
                        Title = $"**{arg.Username} has been muted!**",
                        Description = $"{(string.IsNullOrEmpty(reason) ? "" : $"**Reason: {reason}**")}",
                        Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)))
                    }.WithCurrentTimestamp();
                    sw.Stop();
                    await ReplyAsync(string.Empty, false, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
                    await BaseCommands.sendReasonEmbedToUserDM(arg, Context.Guild, reason, BaseCommands.Commands.Mute);
                }
                else
                {
                    await ReplyAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
                }
            }
        }

        [Command("normie", RunMode = RunMode.Async), Summary("Normies a person."), RequireBotPermission(GuildPermission.KickMembers), RequireUserPermissionAttribute(GuildPermission.KickMembers)]
        public async Task Normie([Summary("User")]SocketGuildUser arg, [Summary("Reason"), Remainder()]string reason = null)
        {
            if (BaseCommands.isVip(Context.User as SocketUser))
            {
                if (jSon.checkPermChn(Context.Guild, Context.Channel.Id, BaseCommands.Commands.Normie))
                {
                    await Context.Message.DeleteAsync();
                    var sw = Stopwatch.StartNew();
                    await arg.RemoveRolesAsync(arg.Roles.Where(x => x.Id != Context.Guild.EveryoneRole.Id && x.Position < x.Guild.CurrentUser.Hierarchy));
                    await arg.AddRolesAsync(arg.Guild.Roles.Where(y => y.Name.ToLower().Equals("normie")));
                    EmbedBuilder builder = new EmbedBuilder()
                    {
                        Title = $"**{arg.Username} has been Normied!**",
                        Description = $"{(string.IsNullOrEmpty(reason) ? "" : $"**Reason: {reason}**")}",
                        Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)))
                    }.WithCurrentTimestamp();
                    sw.Stop();
                    await ReplyAsync(string.Empty, false, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
                    await BaseCommands.sendReasonEmbedToUserDM(arg, Context.Guild, reason, BaseCommands.Commands.Normie);
                }
                else
                {
                    await ReplyAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
                }
            }
        }

        [Command("kick", RunMode = RunMode.Async), Summary("Kicks a person."), RequireBotPermission(GuildPermission.KickMembers), RequireUserPermissionAttribute(GuildPermission.KickMembers)]
        public async Task Kick([Summary("User")]SocketGuildUser arg, [Summary("Reason"), Remainder()]string reason = null)
        {
            if (BaseCommands.isVip(Context.User as SocketUser))
            {
                if (jSon.checkPermChn(Context.Guild, Context.Channel.Id, BaseCommands.Commands.Kick))
                {
                    await Context.Message.DeleteAsync();
                    var sw = Stopwatch.StartNew();
                    BaseCommands.noSend.Add(arg.Id);
                    await BaseCommands.sendReasonEmbedToUserDM(arg, Context.Guild, reason, BaseCommands.Commands.Kick);
                    await arg.KickAsync(reason: reason);
                    EmbedBuilder builder = new EmbedBuilder()
                    {
                        Title = $"**{arg.Username} has been kicked!**",
                        Description = $"{(string.IsNullOrEmpty(reason) ? "" : $"**Reason: {reason}**")}",
                        Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)))
                    }.WithCurrentTimestamp();
                    sw.Stop();
                    await ReplyAsync(string.Empty, false, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
                }
                else
                {
                    await ReplyAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
                }
            }
        }

        [Command("ban", RunMode = RunMode.Async), Summary("Bans a person."), RequireBotPermission(GuildPermission.BanMembers), RequireUserPermissionAttribute(GuildPermission.BanMembers)]
        public async Task Ban([Summary("User")]SocketGuildUser arg, [Summary("Reason"), Remainder()]string reason = null)
        {
            if (BaseCommands.isVip(Context.User as SocketUser))
            {
                if (jSon.checkPermChn(Context.Guild, Context.Channel.Id, BaseCommands.Commands.Ban))
                {
                    await Context.Message.DeleteAsync();
                    var sw = Stopwatch.StartNew();
                    BaseCommands.noSend.Add(arg.Id);
                    await BaseCommands.sendReasonEmbedToUserDM(user: arg, guild: Context.Guild, reason: reason, r: BaseCommands.Commands.Ban);
                    await Context.Guild.AddBanAsync(user: arg, pruneDays: 1, reason: reason);
                    EmbedBuilder builder = new EmbedBuilder()
                    {
                        Title = $"**{arg.Username} has been banned!**",
                        Description = $"{(string.IsNullOrEmpty(reason) ? "" : $"**Reason: {reason}**")}",
                        Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)))
                    }.WithCurrentTimestamp();
                    sw.Stop();
                    await ReplyAsync(string.Empty, false, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
                }
                else
                {
                    await ReplyAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
                }
            }
        }

        [Command("ban", RunMode = RunMode.Async), Summary("Bans a person."), RequireBotPermission(GuildPermission.BanMembers), RequireUserPermissionAttribute(GuildPermission.BanMembers)]
        public async Task Ban([Summary("ID of user")]ulong ID, [Summary("Reason"), Remainder()]string reason = null)
        {
            if (BaseCommands.isVip(Context.User as SocketUser))
            {
                if (jSon.checkPermChn(Context.Guild, Context.Channel.Id, BaseCommands.Commands.Ban))
                {
                    await Context.Message.DeleteAsync();
                    var sw = Stopwatch.StartNew();
                    BaseCommands.noSend.Add(ID);
                    await Context.Guild.AddBanAsync(userId: ID, pruneDays: 1, reason: reason);
                    EmbedBuilder builder = new EmbedBuilder()
                    {
                        Title = $"**{(await Context.Client.GetUserAsync(ID))} has been banned!**",
                        Description = $"{(string.IsNullOrEmpty(reason) ? "" : $"**Reason: {reason}**")}",
                        Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)))
                    }.WithCurrentTimestamp();
                    sw.Stop();
                    await ReplyAsync(string.Empty, false, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
                }
                else
                {
                    await ReplyAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
                }
            }
        }

        [Command("unban", RunMode = RunMode.Async), Summary("Unbans a person."), RequireBotPermission(GuildPermission.BanMembers), RequireUserPermissionAttribute(GuildPermission.BanMembers)]
        public async Task unBan([Summary("ID of user")]ulong ID)
        {
            if (BaseCommands.isVip(Context.User as SocketUser))
            {
                if (jSon.checkPermChn(Context.Guild, Context.Channel.Id, BaseCommands.Commands.Ban))
                {
                    await Context.Message.DeleteAsync();
                    var sw = Stopwatch.StartNew();
                    await Context.Guild.RemoveBanAsync(userId: ID);
                    EmbedBuilder builder = new EmbedBuilder()
                    {
                        Title = $"**{(await Context.Client.GetUserAsync(ID))} has been unbanned!**",
                        Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)))
                    }.WithCurrentTimestamp();
                    sw.Stop();
                    await ReplyAsync(string.Empty, false, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
                }
                else
                {
                    await ReplyAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
                }
            }
        }

        [Command("softban", RunMode = RunMode.Async), Summary("softBans a person."), RequireBotPermission(GuildPermission.BanMembers), RequireUserPermissionAttribute(GuildPermission.BanMembers)]
        public async Task Softban([Summary("User")]SocketGuildUser arg, [Summary("Reason"), Remainder()]string reason = null)
        {
            if (BaseCommands.isVip(Context.User as SocketUser) && (Context.User as SocketGuildUser).GuildPermissions.BanMembers)
            {
                if (jSon.checkPermChn(Context.Guild, Context.Channel.Id, BaseCommands.Commands.Softban))
                {
                    var sw = Stopwatch.StartNew();
                     BaseCommands.noSend.Add(arg.Id);
                    await BaseCommands.sendReasonEmbedToUserDM(arg, Context.Guild, reason, BaseCommands.Commands.Softban);
                    await Context.Guild.AddBanAsync(arg, 5);
                    EmbedBuilder builder = new EmbedBuilder()
                    {
                        Title = $"**{arg.Username} has been softbanned!**",
                        Description = $"{(string.IsNullOrEmpty(reason) ? "" : $"**Reason: {reason}**")}",
                        Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)))
                    }.WithCurrentTimestamp();
                    sw.Stop();
                    await ReplyAsync(string.Empty, false, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
                    await Context.Guild.RemoveBanAsync(arg);
                }
                else
                {
                    await ReplyAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
                }
            }
        }

        [Command("prune", RunMode = RunMode.Async), Summary("Prune messages"), RequireBotPermission(GuildPermission.ManageMessages), RequireUserPermissionAttribute(GuildPermission.ManageMessages)]
        public async Task Prune([Summary("Number of msgs")]int arg1 = 100, [Summary("User")]SocketGuildUser arg2 = null)
        {
            if (BaseCommands.isVip(Context.User as SocketUser))
            {
                if (jSon.checkPermChn(Context.Guild, Context.Channel.Id, BaseCommands.Commands.Prune))
                {
                    var sw = Stopwatch.StartNew();
                    IEnumerable<IMessage> msgs = await Context.Channel.GetMessagesAsync(Context.Message, Direction.Before, arg1).Flatten();
                    if (arg2 != null)
                    {
                        msgs = (await Context.Channel.GetMessagesAsync(Context.Message, Direction.Before, 350).Flatten()).Where(x => x.Author.Id == arg2.Id).Take(arg1);
                    }
                    await Context.Message.DeleteAsync();
                    await Context.Channel.DeleteMessagesAsync(msgs);
                    EmbedBuilder builder = new EmbedBuilder()
                    {
                        Title = $"**{msgs.Count()} messages deleted!**",
                        Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)))
                    }.WithCurrentTimestamp();
                    sw.Stop();
                    await ReplyAsync(string.Empty, false, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
                }
                else
                {
                    await ReplyAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
                }
            }
        }

        [Command("iprune", RunMode = RunMode.Async), Summary("Prune messages"), RequireBotPermission(GuildPermission.ManageMessages), RequireUserPermissionAttribute(GuildPermission.ManageMessages)]
        public async Task iPrune([Summary("Number of msgs")]int arg1 = 10, [Summary("User")]SocketGuildUser arg2 = null)
        {
            if (BaseCommands.isVip(Context.User as SocketUser))
            {
                if (jSon.checkPermChn(Context.Guild, Context.Channel.Id, BaseCommands.Commands.Iprune))
                {
                    var sw = Stopwatch.StartNew();
                    IEnumerable<IMessage> msgs = (await Context.Channel.GetMessagesAsync(Context.Message, Direction.Before, 350).Flatten()).Where(x => x.Attachments.Count != 0).Take(arg1);
                    if (arg2 != null)
                    {
                        msgs = (await Context.Channel.GetMessagesAsync(Context.Message, Direction.Before, 550).Flatten()).Where(x => x.Author.Id == arg2.Id && x.Attachments.Count != 0).Take(arg1);
                    }
                    await Context.Message.DeleteAsync();
                    await Context.Channel.DeleteMessagesAsync(msgs);
                    EmbedBuilder builder = new EmbedBuilder()
                    {
                        Title = $"**{msgs.Count()} messages deleted!**",
                        Color = new Color((byte)(_ran.Next(255)), (byte)(_ran.Next(255)), (byte)(_ran.Next(255)))
                    }.WithCurrentTimestamp();
                    sw.Stop();
                    await ReplyAsync(string.Empty, false, embed: builder.WithFooter(y => y.WithText($"{sw.ElapsedMilliseconds}ms")).Build());
                }
                else
                {
                    await ReplyAsync($"Admin removed this command [Channel:<#{Context.Channel.Id}>]");
                }
            }
        }
    }
}
