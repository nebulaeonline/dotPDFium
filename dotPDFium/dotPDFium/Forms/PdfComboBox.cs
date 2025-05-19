using System;
using nebulae.dotPDFium.Native;

namespace nebulae.dotPDFium.Forms;

public class PdfComboBox : PdfFormElement
{
    internal PdfComboBox(IntPtr annot, int annotIndex, PdfForm form, PdfPage page)
        : base(PdfFormElementType.ComboBox, annot, annotIndex, form, page)
    {
    }

    public int OptionCount => PdfAnnotNative.FPDFAnnot_GetOptionCount(Form.Handle, _annot);

    public string? GetOptionLabel(int index)
    {
        char[] buffer = new char[256];
        uint written = PdfAnnotNative.FPDFAnnot_GetOptionLabel(Form.Handle, _annot, index, buffer, (uint)buffer.Length);
        return written == 0 ? null : new string(buffer, 0, (int)written - 1);
    }

    public int GetSelectedIndex()
    {
        int count = OptionCount;
        for (int i = 0; i < count; i++)
        {
            if (PdfAnnotNative.FPDFAnnot_IsOptionSelected(Form.Handle, _annot, i))
                return i;
        }
        return -1;
    }

    public bool SetSelectedIndex(int index)
    {
        return PdfFormFillNative.FORM_SetIndexSelected(Form.Handle, Page.Handle, index, true);
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
        return (flags & 1) != 0;
    }
}
