using System.IO;

namespace  StreamArray
{
    public class StreamArray/*<T> where T : ISerializable*/
    {
        private readonly Stream _stream;
        private static readonly int BufferSize = 72;
        private readonly byte[] buffer = new byte[BufferSize];
        private long _position =  0;
        private readonly long _length;
        
        public StreamArray(Stream stream)
        {
            _stream = stream;
            _length = _stream.Length;
        }

        public int Read()
        {
            if (_position == _length) return 0;

            int bytesToRead = _length - _position < BufferSize ? (int) (_length - _position) : BufferSize;
            
            int bytesRead = _stream.Read(buffer, 0, bytesToRead);

            _position += bytesRead;

            return bytesRead;
        }
    }

}