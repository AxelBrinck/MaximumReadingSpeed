using System;
using System.IO;
using MaximumSpeed.Utils;

namespace MaximumSpeed
{
    class Program
    {
        private static readonly string FileName = "data.bin";
        private static readonly int TotalValues = 100 * 1024 * 1024;
        private static readonly FileChronometer Chrono = new FileChronometer();
        
        /// <summary>
        /// Writing a file with a conventional BinaryWriter.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="quantity"></param>
        private static void WriteDummyFile(string fileName, int quantity)
        {
            Chrono.Start();
            using(var writer = new BinaryWriter(File.OpenWrite(fileName)))
            {
                for (var value = 0m; value < quantity; value++)
                {
                    writer.Write(value);
                }
            }
            Chrono.End();

            Chrono.Bytes = new FileInfo(fileName).Length;
            Console.WriteLine($"Write Speed: {Chrono.GetBytesPerSecond():0.000}Mb/s. Total: {Chrono.Bytes} bytes.");
        }

        private static unsafe void ReadDummyFile(string fileName)
        {
            var position = 0L;
            decimal last = 0;
            int values = 4096 / sizeof(decimal);
            decimal[] longArray = new decimal[values];
            
            Chrono.Start();
            using(var stream = File.OpenRead(FileName))
            using(var reader = new BinaryReader(stream))
            {
                do
                {
                    var toRead = position + 4096 >= totalBytes ? (int)(totalBytes - position) : 4096;
                    byte[] buffer = reader.ReadBytes(toRead);
                    
                    for (var i = 0; i < toRead; i++)
                    {
                        fixed(byte *pBuffer = buffer)
                        {
                            longArray[i] = *(decimal*) pBuffer + i * sizeof(decimal);
                        }
                    }

                    position += toRead;
                }
                while (position < totalBytes);
            }
            Chrono.End();
            Console.WriteLine(last);

            totalBytes = new FileInfo(FileName).Length;
            elapsedTime = Chrono.GetDuration();
            megaBytes = (float) totalBytes / 1024 / 1024;
            Console.WriteLine($"Read Speed: {megaBytes / elapsedTime.TotalSeconds:0.000}Mb/s. Total: {position} / {totalBytes} bytes.");
        }

        static unsafe void Main(string[] args)
        {
            File.Delete(FileName);
            
            WriteDummyFile(FileName, TotalValues);

            

            
        }
    }
}