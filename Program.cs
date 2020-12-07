using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using MaximumSpeed.Utils;

namespace MaximumSpeed
{
    class Program
    {
        private static readonly string FileName = "data.bin";
        private static readonly int TotalValues = 1024 * 1024 * 100;
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

        private static void ReadDummyFile(string fileName, int elementsToBuffer)
        {
            var position = 0L;
            var fileSize = new FileInfo(fileName).Length;
            var array = new decimal[elementsToBuffer];
            var arraySize = array.Length * sizeof(decimal);
            var elementSize = sizeof(decimal);
            
            Chrono.Start();
            using(var reader = new BinaryReader(File.OpenRead(FileName)))
            {
                do
                {
                    var bytesToRead = position + arraySize >= fileSize ? (int)(fileSize - position) : arraySize;
                    var elementsToRead = bytesToRead / elementSize;
                    var buffer = reader.ReadBytes(bytesToRead);
                    
                    for (var i = 0; i < elementsToRead; i++)
                    {
                        array[i] = new decimal(new int[] {
                            BitConverter.ToInt32(buffer, i * 16),
                            BitConverter.ToInt32(buffer, i * 16 + 4),
                            BitConverter.ToInt32(buffer, i * 16 + 8),
                            BitConverter.ToInt32(buffer, i * 16 + 8)
                        });
                    }

                    position += bytesToRead;
                }
                while (position < fileSize);
            }
            Chrono.End();

            Chrono.Bytes = new FileInfo(fileName).Length;
            Console.WriteLine($"Read Speed: {Chrono.GetBytesPerSecond():0.000}Mb/s. Total: {Chrono.Bytes} bytes.");
        }

        static unsafe void Main(string[] args)
        {
            File.Delete(FileName);
            
            WriteDummyFile(FileName, TotalValues);

            ReadDummyFile(FileName, (1024 * 1024) / sizeof(decimal));
        }
    }
}