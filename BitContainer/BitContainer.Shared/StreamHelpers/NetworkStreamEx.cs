using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace BitContainer.Shared.StreamHelpers
{
    public static class NetworkStreamEx
    {
        private static readonly int _guidSize = Guid.Empty.ToString().Length;
        
        public static async Task ReadBytesAsync(this NetworkStream networkStream, Byte[] bytes)
        {
            await networkStream.ReadAsync(bytes, 0, bytes.Length);
        }

        public static async Task WriteBytesAsync(this NetworkStream networkStream, Byte[] bytes)
        {
            await networkStream.WriteAsync(bytes, 0, bytes.Length);
        }

        public static async Task<Int32> ReadInt32Async(this NetworkStream networkStream)
        {
            Byte[] intBytes = new Byte[sizeof(Int32)];
            await networkStream.ReadBytesAsync(intBytes);
            return BitConverter.ToInt32(intBytes);
        }

        public static async Task<Int64> ReadInt64Async(this NetworkStream networkStream)
        {
            Byte[] intBytes = new Byte[sizeof(Int64)];
            await networkStream.ReadBytesAsync(intBytes);
            return BitConverter.ToInt64(intBytes);
        }

        public static async Task WriteInt64Async(this NetworkStream networkStream, Int64 number)
        {
            Byte[] intBytes = BitConverter.GetBytes(number);
            await networkStream.WriteBytesAsync(intBytes);
        }

        public static async Task<Guid> ReadGuidAsync(this NetworkStream networkStream)
        {
            Byte[] guidBytes = new Byte[_guidSize];
            await networkStream.ReadBytesAsync(guidBytes);
            return new Guid(Encoding.UTF8.GetString(guidBytes));
        }

        public static async Task WriteGuidAsync(this NetworkStream networkStream, Guid guid)
        {
            Byte[] guidBytes = Encoding.UTF8.GetBytes(guid.ToString());
            await networkStream.WriteBytesAsync(guidBytes);
        }

        public static async Task<String> ReadStringAsync(this NetworkStream networkStream)
        {
            Int32 length = await networkStream.ReadInt32Async();
            Byte[] strBytes = new byte[length];
            await networkStream.ReadBytesAsync(strBytes);
            String str = Encoding.UTF8.GetString(strBytes);
            return str;
        }

        public static async Task WriteStringAsync(this NetworkStream networkStream, String str)
        {
            byte[] strBytes = Encoding.UTF8.GetBytes(str);
            byte[] lengthBytes = BitConverter.GetBytes(strBytes.Length);

            await networkStream.WriteAsync(lengthBytes, 0, lengthBytes.Length);
            await networkStream.WriteAsync(strBytes, 0, strBytes.Length);
        }
    }
}
