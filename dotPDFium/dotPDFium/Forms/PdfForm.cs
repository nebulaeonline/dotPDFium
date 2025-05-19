using System;
using nebulae.dotPDFium.Native;

namespace nebulae.dotPDFium.Forms;

public sealed class PdfForm : PdfObject
{
    internal PdfForm(IntPtr handle) : base(handle, PdfObjectType.Form)
    {
        if (handle == IntPtr.Zero)
            throw new dotPDFiumException($"Invalid form handle ({nameof(handle)}): {PdfObject.GetPDFiumError()}");

        _handle = handle;
    }

    public IntPtr Handle => _handle;

    protected override void Dispose(bool disposing)
    {
        // No additional cleanup required for PdfForm at this time.
        base.Dispose(disposing);
    }
}
