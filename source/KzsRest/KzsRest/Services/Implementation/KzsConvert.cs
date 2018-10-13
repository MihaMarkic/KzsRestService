using System;
using KzsRest.Engine.Services.Abstract;

namespace KzsRest.Services.Implementation
{
    public class KzsConvert : IConvert
    {
        public bool TryFromBase64Chars(ReadOnlySpan<char> chars, byte[] buffer, out int bytesWritten)
        {
            if (Convert.TryFromBase64Chars(chars, buffer, out int tempBytesWritten))
            {
                bytesWritten = tempBytesWritten;
                return true;
            }
            else
            {
                bytesWritten = 0;
                return false;
            }
        }
    }
}
