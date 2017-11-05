using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using System.Collections.Generic;
using System.Reflection;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Discord.Commands;

namespace Daddy.Ext
{
    public class FieldValue
    {
        public string Code = null;
        public bool HasCode { get { return Code != null; } }
        public string Content = null;
        public string Title = null;
    }

    public class Extensions
    {
        public static async Task<EmbedBuilder> BuildBotAnswer(Subject<FieldValue> subject)
        {
            var fieldBuilder = new EmbedFieldBuilder().WithIsInline(false);
            var builder = new EmbedBuilder();
            if (subject != null)
            {
                var fields = subject.ToAsyncEnumerable();
                await fields.ForEachAsync(field =>
                {
                    builder.AddField((x) => x.WithName(field.Title).WithValue(field.HasCode ? Format.Code(StripBlock(field.Content), field.Code == null || field.Code.Length == 0 ? null : field.Code) : Format.Sanitize(field.Content)));
                });
            }
            return builder;
        }

        public static string StripBlock(string content)
        {
            return content.Replace("```", "’’’");
        }
    }

    public class Ext
    {
        private int size;
        private Type last;
        private Dictionary<Type, List<object>> buffers;

        public object Last {
            get {
                if (last == null) return null;
                var methods = GetType().GetTypeInfo().GetMethods();
                foreach (var method in methods)
                {
                    if (method.Name.Contains("Peek"))
                    {
                        if (method == null) continue;
                        return method.MakeGenericMethod(last).Invoke(this, null);
                    }
                }
                return null;
            }
        }

        public Ext(int size)
        {
            buffers = new Dictionary<Type, List<object>>();
            this.size = size;
        }

        public void Push<T>(T e)
        {
            if (e == null) return;
            var type = e.GetType();
            last = type;

            foreach (var iface in type.GetTypeInfo().ImplementedInterfaces.Append(type))
            {
                if (!buffers.ContainsKey(iface))
                {
                    buffers[iface] = new List<object>();
                }
                buffers[iface].Insert(0, e);
                if (buffers[iface].Count > size)
                {
                    buffers[iface].RemoveRange(size, buffers[iface].Count - size);
                }
            }
        }

        public IEnumerable<Type> List()
        {
            return buffers.Keys;
        }

        public IEnumerable<T> Inspect<T>()
        {
            var type = typeof(T);
            if (buffers.ContainsKey(type) && buffers[type].Count > 0)
            {
                return (IEnumerable<T>)buffers[type];
            }
            return default(IList<T>);
        }

        public T Peek<T>()
        {
            var type = typeof(T);
            if (buffers.ContainsKey(type) && buffers[type].Count > 0)
            {
                return (T)buffers[type][0];
            }
            return default(T);
        }

        public T Pop<T>()
        {
            var type = typeof(T);
            if (buffers.ContainsKey(type) && buffers[type].Count > 0)
            {
                T item = (T)buffers[type][0];
                buffers[type].RemoveAt(0);
                return item;
            }
            return default(T);
        }
    }

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public sealed class RatelimitAttribute : PreconditionAttribute
    {
        private readonly uint _invokeLimit;
        private readonly bool _noLimitInDMs;
        private readonly bool _noLimitForAdmins;
        private readonly TimeSpan _invokeLimitPeriod;
        private readonly Dictionary<ulong, CommandTimeout> _invokeTracker = new Dictionary<ulong, CommandTimeout>();

        /// <summary> Sets how often a user is allowed to use this command. </summary>
        /// <param name="times">The number of times a user may use the command within a certain period.</param>
        /// <param name="period">The amount of time since first invoke a user has until the limit is lifted.</param>
        /// <param name="measure">The scale in which the <paramref name="period"/> parameter should be measured.</param>
        /// <param name="noLimitInDMs">Set whether or not there is no limit to the command in DMs. Defaults to false.</param>
        /// <param name="noLimitForAdmins">Set whether or not there is no limit to the command for server admins. Defaults to false.</param>
        public RatelimitAttribute(uint times, double period, Measure measure, bool noLimitInDMs = false, bool noLimitForAdmins = false)
        {
            _invokeLimit = times;
            _noLimitInDMs = noLimitInDMs;
            _noLimitForAdmins = noLimitForAdmins;

            //TODO: C# 7 candidate switch expression
            switch (measure)
            {
                case Measure.Days:
                    _invokeLimitPeriod = TimeSpan.FromDays(period);
                    break;
                case Measure.Hours:
                    _invokeLimitPeriod = TimeSpan.FromHours(period);
                    break;
                case Measure.Minutes:
                    _invokeLimitPeriod = TimeSpan.FromMinutes(period);
                    break;
            }
        }

        /// <summary> Sets how often a user is allowed to use this command. </summary>
        /// <param name="times">The number of times a user may use the command within a certain period.</param>
        /// <param name="period">The amount of time since first invoke a user has until the limit is lifted.</param>
        /// <param name="noLimitInDMs">Set whether or not there is no limit to the command in DMs. Defaults to false.</param>
        /// <param name="noLimitForAdmins">Set whether or not there is no limit to the command for server admins. Defaults to false.</param>
        public RatelimitAttribute(uint times, TimeSpan period, bool noLimitInDMs = false, bool noLimitForAdmins = false)
        {
            _invokeLimit = times;
            _noLimitInDMs = noLimitInDMs;
            _noLimitForAdmins = noLimitForAdmins;
            _invokeLimitPeriod = period;
        }

        /// <inheritdoc />
        public override Task<PreconditionResult> CheckPermissions(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            if (_noLimitInDMs && context.Channel is IPrivateChannel)
                return Task.FromResult(PreconditionResult.FromSuccess());

            if (_noLimitForAdmins && context.User is IGuildUser gu && gu.GuildPermissions.Administrator)
                return Task.FromResult(PreconditionResult.FromSuccess());

            var now = DateTime.UtcNow;
            var timeout = (_invokeTracker.TryGetValue(context.User.Id, out var t)
                && ((now - t.FirstInvoke) < _invokeLimitPeriod))
                    ? t : new CommandTimeout(now);

            timeout.TimesInvoked++;

            if (timeout.TimesInvoked <= _invokeLimit)
            {
                _invokeTracker[context.User.Id] = timeout;
                return Task.FromResult(PreconditionResult.FromSuccess());
            }
            else
            {
                return Task.FromResult(PreconditionResult.FromError("You are currently in Timeout."));
            }
        }

        private class CommandTimeout
        {
            public uint TimesInvoked { get; set; }
            public DateTime FirstInvoke { get; }

            public CommandTimeout(DateTime timeStarted)
            {
                FirstInvoke = timeStarted;
            }
        }
    }

    /// <summary> Sets the scale of the period parameter. </summary>
    public enum Measure
    {
        /// <summary> Period is measured in days. </summary>
        Days,

        /// <summary> Period is measured in hours. </summary>
        Hours,

        /// <summary> Period is measured in minutes. </summary>
        Minutes
    }
}
