using System;

namespace MaximumSpeed.Utils
{
    public class Chronometer
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public void Start()
        {
            StartTime = DateTime.Now;
        }

        public void End()
        {
            EndTime = DateTime.Now;
        }

        public TimeSpan GetDuration() => EndTime - StartTime;
    }
}