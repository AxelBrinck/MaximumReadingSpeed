namespace MaximumSpeed.Utils
{
    public class FileChronometer : Chronometer
    {
        public long Bytes { get; set; }

        public double GetBytesPerSecond()
        {
            var megaBytes = (float) Bytes / 1024 / 1024;

            return megaBytes / GetDuration().TotalSeconds;
        }
    }
}