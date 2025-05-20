using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using nebulae.dotPDFium.Native;

namespace nebulae.dotPDFium;

public static class PdfBitmapFormatExtension
{
    /// <summary>
    /// This is a helper method to get the number of bytes per pixel for a given bitmap format.
    /// </summary>
    /// <param name="format"></param>
    /// <returns></returns>
    /// <exception cref="NotSupportedException"></exception>
    public static int GetBytesPerPixel(this PdfBitmapFormat format) => format switch
    {
        PdfBitmapFormat.Gray => 1,
        PdfBitmapFormat.BGR => 3,
        PdfBitmapFormat.BGRx => 4,
        PdfBitmapFormat.BGRA => 4,
        _ => throw new NotSupportedException("Unknown bitmap format")
    };
}
