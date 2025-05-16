using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nebulae.dotPDFium;

public sealed class PdfStandardFont : PdfFont
{
    internal PdfStandardFont(IntPtr handle, string name)
        : base(handle, name) { }

    protected override void Dispose(bool disposing)
    {
        // Do NOT call FPDFFont_Close
        base.Dispose(disposing);
    }
}
