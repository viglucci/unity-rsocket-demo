using System.Collections.Generic;

namespace RSocket
{
    public class RSocketPayload : IPayload
    {
        public List<byte> Data { get; set; }
        public List<byte> Metadata { get; }
    }
}