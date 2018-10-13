using System;

namespace KzsRest.Engine.Services.Abstract
{
    public interface IConvert
    {
        bool TryFromBase64Chars(ReadOnlySpan<char> chars, byte[] buffer, out int bytesWritten);
    }
}
