using System;
using System.Collections.Generic;
using Discord;
using Discord.Commands;
using System.Linq;
using System.Reflection;

namespace Daddy.Modules
{
    public class Globals
    {
        public IEnumerable<Type> _types;
        public IDiscordClient client;
        public SocketCommandContext context;
        public Main.Daddy bot;
        public object l {
            get {
                return bot.ObjectMemory.Last;
            }
        }

        public IEnumerable<Type> ls() => bot.ObjectMemory.List();
        public IEnumerable<T> i<T>() => bot.ObjectMemory.Inspect<T>();

        public T pop<T>()
        {
            return bot.ObjectMemory.Pop<T>();
        }

        public T p<T>()
        {
            return bot.ObjectMemory.Peek<T>();
        }

        public void s<T>(T e)
        {
            bot.ObjectMemory.Push(e);
        }

        public TypeInfo f(string name)
        {
            return _types.SelectMany(x => x.GetTypeInfo().Assembly.GetModules()).SelectMany(x => x.FindTypes(Module.FilterTypeName, name)).FirstOrDefault()?.GetTypeInfo();
        }

    }
}