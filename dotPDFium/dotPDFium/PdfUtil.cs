using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nebulae.dotPDFium;

public static class PdfUtil
{
    public static string ReadUtf8StringFromLengthPrefixedCall(
        Func<UIntPtr, UIntPtr> getLength,
        Func<byte[], UIntPtr, UIntPtr> fillBuffer)
    {
        var length = getLength(UIntPtr.Zero);
        if (length == UIntPtr.Zero) return string.Empty;

        var buffer = new byte[(int)length];
        fillBuffer(buffer, (UIntPtr)buffer.Length);
        return System.Text.Encoding.UTF8.GetString(buffer, 0, buffer.Length - 1); // Exclude null terminator
    }
}
