using nebulae.dotPDFium.Native;

namespace nebulae.dotPDFium.Forms;

public class PdfFormObject : PdfPageObject
{
    internal PdfFormObject(nint handle)
        : base(handle, PdfPageObjectType.Image)
    {
    }
}