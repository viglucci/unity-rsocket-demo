using System.Collections.Generic;

namespace RSocket.Frame
{
    public interface ISerializableFrame<out T>
    {
        public List<byte> Serialize();

        List<byte> SerializeLengthPrefixed();
    }
}