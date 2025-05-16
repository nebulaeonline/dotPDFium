using nebulae.dotPDFium.Native;

namespace nebulae.dotPDFium;

public class PdfPathObject : PdfPageObject
{
    internal PdfPathObject(IntPtr handle)
        : base(handle, PdfPageObjectType.Path)
    {
    }
}