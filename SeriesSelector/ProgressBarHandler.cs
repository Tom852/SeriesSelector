using MediaToolkit;
using System;
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

        public void Start(Series series)
        {
            Length = GetDuration(series);
            T.Start();
            T.Tick += OnTick;
            StartedAt = DateTime.Now;
        }

        private double GetDuration(Series series)
        {
            var inputFile = new MediaToolkit.Model.MediaFile { Filename = series.GetFullFilePathOfCurrentEpisode() };
            using (var engine = new Engine())
            {
                engine.GetMetadata(inputFile);
            }

            return inputFile.Metadata.Duration.TotalSeconds;
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
            T.Tick -= OnTick;
            IsDone?.Invoke(this, null);
        }
    }
}