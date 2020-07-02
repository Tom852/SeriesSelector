using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Threading;

namespace SeriesSelector
{
    public class ProgressBarHandler
    {
        public double Length { get; set; }
        public DispatcherTimer T { get; set; } = new DispatcherTimer();
        private DateTime StartedAt { get; set; }

        public event EventHandler<double> PercentageChanged; 
        public event EventHandler IsDone; 
        public ProgressBarHandler()
        {
            T.Interval = new TimeSpan(0, 0, 1);
        }

        public void Run(double durationInSecs)
        {
            Length = durationInSecs;
            T.Start();
            T.Tick += OnTick;
            StartedAt = DateTime.Now;
        }

        private void OnTick(object sender, EventArgs e)
        {
            TimeSpan diff = DateTime.Now - StartedAt;
            var diff2 = diff.TotalSeconds;
            var percentage = diff2 / Length * 100;
            if (percentage >= 100)
            {
                PercentageChanged?.Invoke(this, 100d);
                Stop();
            }
            else
            {
                PercentageChanged?.Invoke(this, percentage);
            }
        }

        public void Stop()
        {
            T.Stop();
            IsDone?.Invoke(this, null);
        }
    }
}
