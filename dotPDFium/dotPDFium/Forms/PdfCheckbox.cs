using System;
using nebulae.dotPDFium.Native;

namespace nebulae.dotPDFium.Forms;

public class PdfCheckBox : PdfFormElement
{
    internal PdfCheckBox(IntPtr annot, int annotIndex, PdfForm form, PdfPage page)
        : base(PdfFormElementType.CheckBox, annot, annotIndex, form, page)
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
}
