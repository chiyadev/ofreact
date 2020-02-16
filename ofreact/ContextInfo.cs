using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace ofreact
{
    public readonly ref struct ContextInfo<T>
    {
        public readonly ContextInfo Info;

        public T Value
        {
            get
            {
                if (Info?.Value is T value)
                    return value;

                return default;
            }
            set => Info.Value = value;
        }

        ContextInfo(ContextInfo context)
        {
            Info = context;
        }

        public static implicit operator ContextInfo<T>(ContextInfo context) => new ContextInfo<T>(context);
    }

    public sealed class ContextInfo
    {
        readonly HashSet<ofNode> _listeners = new HashSet<ofNode>();

        object _value;

        public object Value
        {
            get => _value;
            set
            {
                var changed = !InternalReflection.PropsEqual(value, _value);

                _value = value;

                if (changed)
                    foreach (var node in _listeners)
                        node.Invalidate();
            }
        }

        internal ContextInfo() { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Subscribe(ofNode node) => _listeners.Add(node);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Unsubscribe(ofNode node) => _listeners.Remove(node);
    }
}