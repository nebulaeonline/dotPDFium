using System;
using System.Runtime.InteropServices;

namespace nebulae.dotPDFium.Native;

public static class PdfFormFillNative
{
    private const string PdfiumLib = "pdfium";

    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDFDOC_InitFormFillEnvironment(IntPtr doc, ref PdfFormFillInfo formInfo);

    [DllImport(PdfiumLib)]
    public static extern void FPDFDOC_ExitFormFillEnvironment(IntPtr formHandle);

    [DllImport(PdfiumLib)]
    public static extern void FORM_OnAfterLoadPage(IntPtr page, IntPtr formHandle);

    [DllImport(PdfiumLib)]
    public static extern void FORM_OnBeforeClosePage(IntPtr page, IntPtr formHandle);

    [DllImport(PdfiumLib)]
    public static extern void FORM_DoDocumentJSAction(IntPtr formHandle);

    [DllImport(PdfiumLib)]
    public static extern void FORM_DoDocumentOpenAction(IntPtr formHandle);

    [DllImport(PdfiumLib)]
    public static extern void FPDF_FFLDraw(
        IntPtr formHandle,
        IntPtr bitmap,
        IntPtr page,
        int startX,
        int startY,
        int sizeX,
        int sizeY,
        int rotate,
        int flags);

    [DllImport(PdfiumLib)]
    public static extern int FPDF_GetFormType(IntPtr document);

    [DllImport(PdfiumLib)]
    public static extern bool FPDF_LoadXFA(IntPtr document);

    [DllImport(PdfiumLib)]
    public static extern bool FORM_OnFocus(IntPtr formHandle, IntPtr page, int modifier, double x, double y);

    [DllImport(PdfiumLib)]
    public static extern bool FORM_OnLButtonDown(IntPtr formHandle, IntPtr page, int modifier, double x, double y);

    [DllImport(PdfiumLib)]
    public static extern bool FORM_OnLButtonUp(IntPtr formHandle, IntPtr page, int modifier, double x, double y);

    [DllImport(PdfiumLib)]
    public static extern bool FORM_OnChar(IntPtr formHandle, IntPtr page, int charCode, int modifier);

    [DllImport(PdfiumLib)]
    public static extern bool FORM_OnKeyDown(IntPtr formHandle, IntPtr page, int keyCode, int modifier);

    [DllImport(PdfiumLib)]
    public static extern bool FORM_OnKeyUp(IntPtr formHandle, IntPtr page, int keyCode, int modifier);

    [DllImport(PdfiumLib)]
    public static extern void FORM_DoDocumentAAction(IntPtr formHandle, int aaType);

    [DllImport(PdfiumLib)]
    public static extern void FORM_DoPageAAction(IntPtr page, IntPtr formHandle, int aaType);

    [DllImport(PdfiumLib)]
    public static extern bool FORM_OnMouseMove(IntPtr formHandle, IntPtr page, int modifier, double x, double y);

    [DllImport(PdfiumLib)]
    public static extern bool FORM_OnMouseWheel(IntPtr formHandle, IntPtr page, int modifier, ref FsPointF coord, int deltaX, int deltaY);

    [DllImport(PdfiumLib)]
    public static extern bool FORM_OnRButtonDown(IntPtr formHandle, IntPtr page, int modifier, double x, double y);

    [DllImport(PdfiumLib)]
    public static extern bool FORM_OnRButtonUp(IntPtr formHandle, IntPtr page, int modifier, double x, double y);

    [DllImport(PdfiumLib)]
    public static extern bool FORM_OnLButtonDoubleClick(IntPtr formHandle, IntPtr page, int modifier, double x, double y);

    [DllImport(PdfiumLib)]
    public static extern uint FORM_GetFocusedText(IntPtr formHandle, IntPtr page, IntPtr buffer, uint buflen);

    [DllImport(PdfiumLib)]
    public static extern uint FORM_GetSelectedText(IntPtr formHandle, IntPtr page, IntPtr buffer, uint buflen);

    [DllImport(PdfiumLib)]
    public static extern void FORM_ReplaceAndKeepSelection(IntPtr formHandle, IntPtr page, [MarshalAs(UnmanagedType.LPWStr)] string text);

    [DllImport(PdfiumLib)]
    public static extern void FORM_ReplaceSelection(IntPtr formHandle, IntPtr page, [MarshalAs(UnmanagedType.LPWStr)] string text);

    [DllImport(PdfiumLib)]
    public static extern bool FORM_SelectAllText(IntPtr formHandle, IntPtr page);

    [DllImport(PdfiumLib)]
    public static extern bool FORM_CanUndo(IntPtr formHandle, IntPtr page);

    [DllImport(PdfiumLib)]
    public static extern bool FORM_CanRedo(IntPtr formHandle, IntPtr page);

    [DllImport(PdfiumLib)]
    public static extern bool FORM_Undo(IntPtr formHandle, IntPtr page);

    [DllImport(PdfiumLib)]
    public static extern bool FORM_Redo(IntPtr formHandle, IntPtr page);

    [DllImport(PdfiumLib)]
    public static extern bool FORM_ForceToKillFocus(IntPtr formHandle);

    [DllImport(PdfiumLib)]
    public static extern bool FORM_GetFocusedAnnot(IntPtr formHandle, out int pageIndex, out IntPtr annot);

    [DllImport(PdfiumLib)]
    public static extern bool FORM_SetFocusedAnnot(IntPtr formHandle, IntPtr annot);

    [DllImport(PdfiumLib)]
    public static extern int FPDFPage_HasFormFieldAtPoint(IntPtr formHandle, IntPtr page, double x, double y);

    [DllImport(PdfiumLib)]
    public static extern int FPDFPage_FormFieldZOrderAtPoint(IntPtr formHandle, IntPtr page, double x, double y);

    [DllImport(PdfiumLib)]
    public static extern void FPDF_SetFormFieldHighlightColor(IntPtr formHandle, int fieldType, uint color);

    [DllImport(PdfiumLib)]
    public static extern void FPDF_SetFormFieldHighlightAlpha(IntPtr formHandle, byte alpha);

    [DllImport(PdfiumLib)]
    public static extern void FPDF_RemoveFormFieldHighlight(IntPtr formHandle);

    [DllImport(PdfiumLib)]
    public static extern bool FORM_SetIndexSelected(IntPtr formHandle, IntPtr page, int index, bool selected);

    [DllImport(PdfiumLib)]
    public static extern bool FORM_IsIndexSelected(IntPtr formHandle, IntPtr page, int index);
}

