using System;
using Server.Commands;
using System.Reflection;

namespace Server
{
    public delegate void OnPropertyChangedEventHandler(OnPropertyChangedEventArgs e);

    public static partial class EventSink
    {
        public static event CommandEventHandler Command;
        public static event OnPropertyChangedEventHandler OnPropertyChanged;

        public static void InvokeCommand(CommandEventArgs e)
        {
            Command?.Invoke(e);
        }

        public static void InvokeOnPropertyChanged(OnPropertyChangedEventArgs e)
        {
            OnPropertyChanged?.Invoke(e);
        }

        private static void ResetCommands()
        {
            Command = null;
            OnPropertyChanged = null;
        }
    }

    public class OnPropertyChangedEventArgs : EventArgs
    {
        public Mobile Mobile { get; private set; }
        public PropertyInfo Property { get; private set; }
        public object Instance { get; private set; }
        public object OldValue { get; private set; }
        public object NewValue { get; private set; }

        public OnPropertyChangedEventArgs(Mobile m, object instance, PropertyInfo prop, object oldValue, object newValue)
        {
            Mobile = m;
            Property = prop;
            Instance = instance;
            OldValue = oldValue;
            NewValue = newValue;
        }
    }
}
