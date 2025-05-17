using nebulae.dotPDFium.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nebulae.dotPDFium;

public static class PdfRuntime
{
    /// <summary>
    /// Registers a handler for unsupported PDF objects such as Type3 fonts, XFA, or advanced features.
    /// This must be set before opening documents.
    /// </summary>
    /// <param name="info">The handler info struct with a function pointer.</param>
    public static void SetUnsupportedObjectHandler(UnSupportInfo info)
    {
        if (!PdfExtNative.FSDK_SetUnSpObjProcessHandler(ref info))
            throw new dotPDFiumException("Failed to set unsupported object handler.");
    }
}
