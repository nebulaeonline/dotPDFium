using System;
using nebulae.dotPDFium.Native;

namespace nebulae.dotPDFium.Forms;

public class PdfSignatureField : PdfFormElement
{
    internal PdfSignatureField(IntPtr annot, int annotIndex, PdfForm form, PdfPage page)
        : base(PdfFormElementType.SignatureField, annot, annotIndex, form, page)
    {
    }

    public bool IsSigned()
    {
        // PDFium does not provide rich signature APIs; a non-empty value implies signed
        char[] buffer = new char[512];
        uint written = PdfAnnotNative.FPDFAnnot_GetFormFieldValue(Form.Handle, _annot, buffer, (uint)buffer.Length);
        return written > 1; // non-empty string (excluding null terminator)
    }

    public string? GetSignatureValue()
    {
        char[] buffer = new char[512];
        uint written = PdfAnnotNative.FPDFAnnot_GetFormFieldValue(Form.Handle, _annot, buffer, (uint)buffer.Length);
        return written == 0 ? null : new string(buffer, 0, (int)written - 1);
    }

    public bool IsReadOnly()
    {
        int flags = PdfAnnotNative.FPDFAnnot_GetFormFieldFlags(Form.Handle, _annot);
        return (flags & 1) != 0;
    }
}
