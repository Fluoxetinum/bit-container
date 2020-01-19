using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BitContainer.Shared.StreamHelpers
{
    public static class StreamEx
    {
        private static readonly int _blockSize = 10000;

        public static async Task WriteToStream(Stream fromStream, Stream toStream, Int64 count)
        {
            Int64 hasRead = 0;
            Byte[] data = new byte[_blockSize];

            while (hasRead < count)
            {
                Int64 remainder = count - hasRead;

                Boolean remainderLessThanBlock = remainder < _blockSize;
                Int32 block = remainderLessThanBlock ? (Int32) remainder : _blockSize;

                Int32 readedCount = await fromStream.ReadAsync(data, 0, block);
                hasRead += readedCount;
                await toStream.WriteAsync(data, 0, readedCount);
            }
        }
    }
}
