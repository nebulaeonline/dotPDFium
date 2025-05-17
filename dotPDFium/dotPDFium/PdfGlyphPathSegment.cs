using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nebulae.dotPDFium;

public readonly struct PdfGlyphPathSegment
{
    public IntPtr Handle { get; }

    internal PdfGlyphPathSegment(IntPtr handle)
    {
        Handle = handle;
    }

    // Later: you can expose segment type, point, etc. via additional API
}

