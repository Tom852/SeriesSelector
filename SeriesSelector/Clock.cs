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


        public Clock()
        {
            timer.Interval = new TimeSpan(0, 0, 10);
            timer.Tick += (sender, args) => OnTimechange?.Invoke(this, GetTime());
            timer.Start();
        }
        public string GetTime()
        {
            DateTime now = DateTime.Now;
            return now.ToString("HH:mm");
        }

        ~Clock()
        {
            timer.Stop();
        }
    }
}
