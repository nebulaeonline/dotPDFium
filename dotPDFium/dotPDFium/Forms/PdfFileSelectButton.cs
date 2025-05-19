using System;
using nebulae.dotPDFium.Native;

namespace nebulae.dotPDFium.Forms;

public class PdfFileSelectButton : PdfFormElement
{
    internal PdfFileSelectButton(IntPtr annot, int annotIndex, PdfForm form, PdfPage page)
        : base(PdfFormElementType.FileSelectButton, annot, annotIndex, form, page)
    {
    }

    public bool IsReadOnly()
    {
        int flags = PdfAnnotNative.FPDFAnnot_GetFormFieldFlags(Form.Handle, _annot);
        return (flags & 1) != 0;
    }

    public bool IsFileSelectField()
    {
        int flags = PdfAnnotNative.FPDFAnnot_GetFormFieldFlags(Form.Handle, _annot);
        return (flags & (1 << 20)) != 0;
    }
}
