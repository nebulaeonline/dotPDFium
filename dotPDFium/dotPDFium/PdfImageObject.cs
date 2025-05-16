using nebulae.dotPDFium.Native;

namespace nebulae.dotPDFium;

public class PdfImageObject : PdfPageObject
{
    internal PdfImageObject(IntPtr handle)
        : base(handle, PdfPageObjectType.Image)
    {
    }    
}