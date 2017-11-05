using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace AniFile3
{
    public class ScheduleTask
    {
        private List<Timer> _timers;

        public ScheduleTask()
        {
            _timers = new List<Timer>();
        }

        public void Start(int ms, Action action, Func<bool> canceler = null)
        {
            var timer = new Timer(ms);
            timer.Elapsed += (sender, e) =>
            {
                if(canceler != null && canceler())
                {
                    timer.Stop();
                    _timers.Remove(timer);
                }

                action();
            };
            timer.Start();

            _timers.Add(timer);
        }
    }
}
