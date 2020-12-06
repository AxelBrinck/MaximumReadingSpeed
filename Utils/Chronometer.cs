using System;

namespace MaximumSpeed.Utils
{
    public class Chronometer
    {
        private DateTime _started;
        private DateTime _finished;

        public void Start() => _started = DateTime.Now;

        public void End() => _finished = DateTime.Now;

        public TimeSpan GetDuration() => _finished - _started;
    }
}