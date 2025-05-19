using nebulae.dotPDFium.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nebulae.dotPDFium.Forms;

internal static class PdfFormElementFactory
{
    public static PdfFormElementType DetermineElementType(IntPtr formHandle, IntPtr annot)
    {
        int type = PdfAnnotNative.FPDFAnnot_GetFormFieldType(formHandle, annot);
        return type switch
        {
            0 => PdfFormElementType.PushButton,
            1 => PdfFormElementType.CheckBox,
            2 => PdfFormElementType.RadioButton,
            3 => PdfFormElementType.ComboBox,
            4 => PdfFormElementType.ListBox,
            5 => PdfFormElementType.TextField,
            6 => PdfFormElementType.SignatureField,
            _ => PdfFormElementType.Unknown
        };
    }

    public static PdfFormElement CreateElement(
        PdfFormElementType type,
        IntPtr annot,
        int index,
        PdfForm form,
        PdfPage page)
    {
        return type switch
        {
            PdfFormElementType.TextField => new PdfTextField(annot, index, form, page),
            PdfFormElementType.CheckBox => new PdfCheckBox(annot, index, form, page),
            PdfFormElementType.RadioButton => new PdfRadioButton(annot, index, form, page),
            PdfFormElementType.ComboBox => new PdfComboBox(annot, index, form, page),
            PdfFormElementType.ListBox => new PdfListBox(annot, index, form, page),
            PdfFormElementType.PushButton => new PdfPushButton(annot, index, form, page),
            PdfFormElementType.SignatureField => new PdfSignatureField(annot, index, form, page),
            _ => throw new NotSupportedException($"Unsupported form element type: {type}")
        };
    }
}

