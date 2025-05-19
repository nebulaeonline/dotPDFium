using System;
using nebulae.dotPDFium.Native;

namespace nebulae.dotPDFium.Forms;

public class PdfTextField : PdfFormElement
{
    internal PdfTextField(IntPtr annot, int annotIndex, PdfForm form, PdfPage page)
        : base(PdfFormElementType.TextField, annot, annotIndex, form, page)
    {
    }

    public string? GetValue()
    {
        char[] buffer = new char[512];
        uint written = PdfAnnotNative.FPDFAnnot_GetFormFieldValue(Form.Handle, _annot, buffer, (uint)buffer.Length);
        return written == 0 ? null : new string(buffer, 0, (int)written - 1);
    }

    public bool SetValue(string value)
    {
        return PdfAnnotNative.FPDFAnnot_SetStringValue(_annot, "V", value);
    }

    public bool IsReadOnly()
    {
        int flags = PdfAnnotNative.FPDFAnnot_GetFormFieldFlags(Form.Handle, _annot);
        return (flags & 1) != 0; // bit 1 == ReadOnly
    }
}
