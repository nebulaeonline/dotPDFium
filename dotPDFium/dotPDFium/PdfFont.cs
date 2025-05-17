using System.Runtime.InteropServices;

namespace nebulae.dotPDFium;

public class PdfFont : PdfObject
{
    private readonly string _name;

    internal PdfFont(IntPtr handle, string name)
        : base(handle, PdfObjectType.Font)
    {
        _name = name;
    }

    public string Name => _name;
}
