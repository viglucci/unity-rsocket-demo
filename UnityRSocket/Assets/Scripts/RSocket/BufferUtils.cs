using System;
using System.Collections.Generic;

namespace RSocket
{
    public static class BufferUtils
    {
        public static void WriteUInt32BigEndian(List<byte> target, Int32 value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            target.AddRange(bytes);
        }

        public static void WriteUInt16BigEndian(List<byte> target, int value)
        {
            WriteUInt16BigEndian(target, (short) value);
        }
        
        public static void WriteUInt16BigEndian(List<byte> target, short value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(bytes);
            }

            target.AddRange(bytes);
        }
        
        public static void WriteInt8(List<byte> target, byte value)
        {
            byte[] bytes = {value};
            target.AddRange(bytes);
        }

        public static void WriteUInt24BigEndian(List<byte> target, int value)
        {
            UInt24 length = new UInt24((uint) value);
            target.AddRange(length.BytesBigEndian);
        }
    }
}