using System;
using nebulae.dotPDFium.Native;

namespace nebulae.dotPDFium.Forms;

public class PdfPushButton : PdfFormElement
{
    internal PdfPushButton(IntPtr annot, int annotIndex, PdfForm form, PdfPage page)
        : base(PdfFormElementType.PushButton, annot, annotIndex, form, page)
    {
    }

    public string? GetCaption()
    {
        char[] buffer = new char[256];
        uint written = PdfAnnotNative.FPDFAnnot_GetFormFieldValue(Form.Handle, _annot, buffer, (uint)buffer.Length);
        return written == 0 ? null : new string(buffer, 0, (int)written - 1);
    }

    public bool IsReadOnly()
    {
        int flags = PdfAnnotNative.FPDFAnnot_GetFormFieldFlags(Form.Handle, _annot);
        return (flags & 1) != 0;
    }

    public int GetControlIndex()
    {
        return PdfAnnotNative.FPDFAnnot_GetFormControlIndex(Form.Handle, _annot);
    }

    public int GetControlCount()
    {
        return PdfAnnotNative.FPDFAnnot_GetFormControlCount(Form.Handle, _annot);
    }
}
