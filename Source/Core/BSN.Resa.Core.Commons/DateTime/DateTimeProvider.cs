using System;
using System.Collections;
using System.Threading;

namespace BSN.Resa.Core.Commons
{
    public class DateTimeProvider
    {
        public static DateTime Now
            => DateTimeProviderContext.Current == null
                    ? DateTime.Now
                    : DateTimeProviderContext.Current.ContextDateTimeNow;

        public static DateTime UtcNow => Now.ToUniversalTime();

        public static DateTime Today => Now.Date;
    }


    public class DateTimeProviderContext : IDisposable
    {
        internal DateTime ContextDateTimeNow;
        private static readonly ThreadLocal<Stack> ThreadScopeStack = new ThreadLocal<Stack>(() => new Stack());

        public DateTimeProviderContext(DateTime contextDateTimeNow)
        {
            ContextDateTimeNow = contextDateTimeNow;
            ThreadScopeStack.Value.Push(this);
        }

        public static DateTimeProviderContext Current
        {
            get
            {
                if (ThreadScopeStack.Value.Count == 0)
                    return null;
                else
                    return ThreadScopeStack.Value.Peek() as DateTimeProviderContext;
            }
        }

        public void Dispose()
        {
            ThreadScopeStack.Value.Pop();
        }
    }
}
