using nebulae.dotPDFium.Native;

namespace nebulae.dotPDFium;

public class PdfFormObject : PdfPageObject
{
    internal PdfFormObject(IntPtr handle)
        : base(handle, PdfPageObjectType.Image)
    {
    }
}