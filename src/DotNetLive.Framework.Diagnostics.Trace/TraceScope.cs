using System;
using System.Threading;

namespace DotNetLive.Framework.Diagnostics.Trace
{
    internal class TraceScope
    {
        private readonly string _name;
        private readonly object _state;

        public TraceScope(string name, object state)
        {
            _name = name;
            _state = state;
        }

        public ActivityContext Context { get; set; }

        public TraceScope Parent { get; set; }

        public ScopeNode Node { get; set; }

        public bool LogEnabled { get; set; }

        private static AsyncLocal<TraceScope> _value = new AsyncLocal<TraceScope>();
        public static TraceScope Current
        {
            set
            {
                _value.Value = value;
            }
            get
            {
                return _value.Value;
            }
        }

        public static IDisposable Push(TraceScope scope, TraceStore store)
        {
            if (scope == null)
            {
                throw new ArgumentNullException(nameof(scope));
            }

            if (store == null)
            {
                throw new ArgumentNullException(nameof(store));
            }

            //if (scope._name.Equals("Microsoft.AspNetCore.Hosting.Internal.WebHost", StringComparison.OrdinalIgnoreCase))
            //{
            //    return new DisposableAction(() =>
            //    {
            //        //Current.Node.EndTime = DateTimeOffset.UtcNow;
            //        //Current = Current.Parent;
            //    });
            //}

            var temp = Current;
            Current = scope;
            Current.Parent = temp;

            Current.Node = new ScopeNode()
            {
                StartTime = DateTimeOffset.UtcNow,
                State = Current._state,
                Name = Current._name
            };

            if (Current.Parent != null)
            {
                Current.Node.Parent = Current.Parent.Node;
                Current.Parent.Node.Children.Add(Current.Node);
            }
            else
            {
                Current.Context.Root = Current.Node;
                store.AddActivity(Current.Context);
            }

            return new DisposableAction(() =>
            {
                Current.Node.EndTime = DateTimeOffset.UtcNow;
                Current = Current.Parent;
            });
        }

        private class DisposableAction : IDisposable
        {
            private Action _action;

            public DisposableAction(Action action)
            {
                _action = action;
            }

            public void Dispose()
            {
                if (_action != null)
                {
                    _action.Invoke();
                    _action = null;
                }
            }
        }
    }
}