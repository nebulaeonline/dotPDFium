using nebulae.dotPDFium.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nebulae.dotPDFium;

public sealed class PdfEmbeddedFont : PdfFont
{
    internal PdfEmbeddedFont(IntPtr handle, string name)
        : base(handle, name) { }

    protected override void Dispose(bool disposing)
    {
        if (_handle != IntPtr.Zero)
            PdfEditNative.FPDFFont_Close(_handle);

        base.Dispose(disposing);
    }
}
