using System;
using System.Collections.Generic;
using System.Windows.Threading;

namespace AniFile3
{
    public class ScheduleTask : IDisposable
    {
        private List<DispatcherTimer> _timers;

        public ScheduleTask()
        {
            _timers = new List<DispatcherTimer>();
        }

        public void Dispose()
        {
            foreach (var timer in _timers)
                timer.Stop();
            _timers.Clear();
        }

        public void Start(int ms, Action action, Func<bool> canceler = null)
        {
            var timer = new DispatcherTimer();
            timer.Tick += (sender, e) =>
            {
                if (canceler != null && canceler())
                {
                    timer.Stop();
                    _timers.Remove(timer);
                }

                action();
            };
            
            timer.Interval = TimeSpan.FromMilliseconds(ms);
            timer.Start();

            _timers.Add(timer);
        }
    }
}
