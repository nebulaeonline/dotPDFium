using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace nebulae.dotPDFium;

internal static class PdfUtil
{
    internal static string ReadUtf8StringFromLengthPrefixedCall(
        Func<UIntPtr, UIntPtr> getLength,
        Func<byte[], UIntPtr, UIntPtr> fillBuffer)
    {
        var length = getLength(UIntPtr.Zero);
        if (length == UIntPtr.Zero) return string.Empty;

        var buffer = new byte[(int)length];
        fillBuffer(buffer, (UIntPtr)buffer.Length);
        return System.Text.Encoding.UTF8.GetString(buffer, 0, buffer.Length - 1); // Exclude null terminator
    }

    internal delegate bool Utf8Reader(IntPtr buffer, uint bufferLength, out uint actualLength);

    internal static string? ReadUtf8(Utf8Reader call)
    {
        call(IntPtr.Zero, 0, out uint length);
        if (length == 0) return null;

        IntPtr buffer = Marshal.AllocHGlobal((int)length);
        try
        {
            if (!call(buffer, length, out _))
                return null;

            return Marshal.PtrToStringUTF8(buffer);
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }

    internal static string? ReadUtf16(Func<IntPtr, int, int> call)
    {
        int requiredChars = call(IntPtr.Zero, 0);
        if (requiredChars <= 0)
            return null;

        IntPtr buffer = Marshal.AllocHGlobal(requiredChars * sizeof(char));
        try
        {
            int written = call(buffer, requiredChars);
            if (written <= 0)
                return null;

            return Marshal.PtrToStringUni(buffer, written);
        }
        finally
        {
            Marshal.FreeHGlobal(buffer);
        }
    }
}
