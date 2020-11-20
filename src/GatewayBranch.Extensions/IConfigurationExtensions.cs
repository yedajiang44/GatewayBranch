using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GatewayBranch.Extensions
{
    public static class IConfigurationExtensions
    {
        /// <summary>
        /// Perform an action when configuration changes. Note this requires config sources to be added with
        /// `reloadOnChange` enabled
        /// </summary>
        /// <param name="config">Configuration to watch for changes</param>
        /// <param name="action">Action to perform when <paramref name="config"/> is changed</param>
        /// <param name="delayInMilliseconds">Delay In Milliseconds</param>
        public static void OnChange(this IConfiguration config, Action action, int delayInMilliseconds = 500)
        {
            // IConfiguration's change detection is based on FileSystemWatcher, which will fire multiple change
            // events for each change - Microsoft's code is buggy in that it doesn't bother to debounce/dedupe
            // https://github.com/aspnet/AspNetCore/issues/2542
            var debouncer = new Debouncer(TimeSpan.FromMilliseconds(delayInMilliseconds));

            ChangeToken.OnChange<object>(config.GetReloadToken, _ => debouncer.Debouce(action), null);
        }
        private class Debouncer : IDisposable
        {
            private readonly CancellationTokenSource cts = new CancellationTokenSource();
            private readonly TimeSpan waitTime;
            private int counter;

            public Debouncer(TimeSpan? waitTime = null)
            {
                this.waitTime = waitTime ?? TimeSpan.FromSeconds(3);
            }

            public void Debouce(Action action)
            {
                var current = Interlocked.Increment(ref counter);

                Task.Delay(waitTime).ContinueWith(task =>
                {
                    // Is this the last task that was queued?
                    if (current == counter && !cts.IsCancellationRequested)
                        action();

                    task.Dispose();
                }, cts.Token);
            }

            public void Dispose()
            {
                cts.Cancel();
            }
        }
    }
}
