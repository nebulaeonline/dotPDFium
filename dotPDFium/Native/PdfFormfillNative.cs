using Nebulae.dotPDFium.Native;
using System;
using System.Runtime.InteropServices;

namespace nebulae.dotPDFium.Native;

public static class PdfFormNative
{
    private const string PdfiumLib = "pdfium";

    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDFDOC_InitFormFillEnvironment(IntPtr doc, ref FPDF_FORMFILLINFO formInfo);

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
}

