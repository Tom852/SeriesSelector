using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace SeriesSelector
{
    public class Clock
    {
        public event EventHandler<string> OnTimechange;
        private readonly DispatcherTimer timer = new DispatcherTimer();
        public string Time { get; private set; }


        public Clock()
        {
            RefreshTime();
            timer.Interval = new TimeSpan(0, 0, 5);
            timer.Tick += (sender, args) =>
            {
                if (!TimeHasChanged()) return;
                RefreshTime();
                OnTimechange?.Invoke(this, Time);
            };
            timer.Start();
        }
        public string GetTime()
        {
            DateTime now = DateTime.Now;
            return now.ToString("HH:mm");
        }

        private void RefreshTime()
        {
            Time = GetTime();
        }

        private bool TimeHasChanged()
        {
            string oldTime = Time;
            string newTime = GetTime();
            return oldTime != newTime;
        }


        ~Clock()
        {
            timer.Stop();
        }
    }
}
