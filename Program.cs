﻿using System;
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

        private static unsafe void ReadDummyFile(string fileName, int elementsToBuffer)
        {
            var position = 0L;
            var fileSize = new FileInfo(fileName).Length;
            var array = new long[elementsToBuffer];
            var arraySize = array.Length * sizeof(long);
            var elementSize = sizeof(long);
            var converters = new List<Task>();
            var processors = Environment.ProcessorCount;
            var semaphore = new Semaphore(processors - 4, processors - 4);
            
            Chrono.Start();
            using(var reader = new BinaryReader(File.OpenRead(FileName)))
            {
                do
                {
                    var bytesToRead = position + arraySize >= fileSize ? (int)(fileSize - position) : arraySize;
                    var elementsToRead = bytesToRead / elementSize;
                    var buffer = reader.ReadBytes(bytesToRead);
                    Thread.CurrentThread.Priority = ThreadPriority.Highest;
                    var task = Task.Factory.StartNew(() => {
                        //Console.WriteLine("Task started");
                        var array = new decimal[elementsToRead];
                        for (var i = 0; i < elementsToRead; i++)
                        {
                            semaphore.WaitOne();
                            fixed(byte *pBuffer = buffer)
                            {
                                array[i] = new decimal(new int[] {
                                    *(int*) (pBuffer + i * elementSize),
                                    *(int*) (pBuffer + i * elementSize + 4),
                                    *(int*) (pBuffer + i * elementSize + 8),
                                    *(int*) (pBuffer + i * elementSize + 12)
                                });
                            }
                            semaphore.Release();
                        }
                        //Console.WriteLine("Task ended");
                    });
                    converters.Add(task);

                    position += bytesToRead;
                }
                while (position < fileSize);
            }
            Chrono.End();

            Task.WhenAll(converters);

            Chrono.Bytes = new FileInfo(fileName).Length;
            Console.WriteLine($"Read Speed: {Chrono.GetBytesPerSecond():0.000}Mb/s. Total: {Chrono.Bytes} bytes.");
            Console.WriteLine($"Read Speed: {Chrono.GetBytesPerSecond() / 16:0.000}MDecimals/s. Total: {Chrono.Bytes} bytes.");
        }

        static unsafe void Main(string[] args)
        {
            //File.Delete(FileName);
            
            //WriteDummyFile(FileName, TotalValues);

            ReadDummyFile(FileName, (1024 * 1024) / sizeof(decimal));
        }
    }
}