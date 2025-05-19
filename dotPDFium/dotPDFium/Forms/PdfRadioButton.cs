using System;
using nebulae.dotPDFium.Native;

namespace nebulae.dotPDFium.Forms;

public class PdfRadioButton : PdfFormElement
{
    internal PdfRadioButton(IntPtr annot, int annotIndex, PdfForm form, PdfPage page)
        : base(PdfFormElementType.RadioButton, annot, annotIndex, form, page)
    {
    }

    public bool IsChecked()
    {
        return PdfAnnotNative.FPDFAnnot_IsChecked(Form.Handle, _annot);
    }

    public bool SetChecked(bool value)
    {
        string state = value ? "Yes" : "Off";
        return PdfAnnotNative.FPDFAnnot_SetStringValue(_annot, "V", state);
    }

    public bool IsReadOnly()
    {
        int flags = PdfAnnotNative.FPDFAnnot_GetFormFieldFlags(Form.Handle, _annot);
        return (flags & 1) != 0;
    }

    public string? GetExportValue()
    {
        char[] buffer = new char[256];
        uint written = PdfAnnotNative.FPDFAnnot_GetFormFieldExportValue(Form.Handle, _annot, buffer, (uint)buffer.Length);
        return written == 0 ? null : new string(buffer, 0, (int)written - 1);
    }
}

