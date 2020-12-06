using System;
using System.IO;
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
                for (var value = 0L; value < quantity; value++)
                {
                    writer.Write(value);
                }
            }
            Chrono.End();

            Chrono.Bytes = new FileInfo(fileName).Length;
            Console.WriteLine($"Write Speed: {Chrono.GetBytesPerSecond():0.000}Mb/s. Total: {Chrono.Bytes} bytes.");
        }

        private static unsafe void ReadDummyFile(string fileName, int elementsToBuffer)
        {
            var position = 0L;
            var fileSize = new FileInfo(fileName).Length;
            var array = new long[elementsToBuffer];
            var arraySize = array.Length * sizeof(long);
            var elementSize = sizeof(long);
            
            Chrono.Start();
            using(var reader = new BinaryReader(File.OpenRead(FileName)))
            {
                do
                {
                    var bytesToRead = position + arraySize >= fileSize ? (int)(fileSize - position) : arraySize;
                    var elementsToRead = bytesToRead / elementSize;
                    byte[] buffer = reader.ReadBytes(bytesToRead);
                    
                    for (var i = 0; i < elementsToRead; i++)
                    {
                        fixed(byte *pBuffer = buffer)
                        {
                            array[i] = *(long*) (pBuffer + i * elementSize);
                        }
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

            ReadDummyFile(FileName, 4096 / sizeof(long));

            
        }
    }
}