using System;

namespace RSocket
{
    public class RSocketError : Exception
    {
        public RSocketErrorCodes Code { get; }

        public RSocketError(RSocketErrorCodes code, string errorMessage) : base(errorMessage)
        {
            Code = code;
        }
    }
}