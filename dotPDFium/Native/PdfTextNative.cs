using System;
using System.Runtime.InteropServices;

namespace nebulae.dotPDFium.Native;
public static class PdfTextNative
{
    private const string PdfiumLib = "pdfium";
    [DllImport(PdfiumLib)]
    public static extern void FPDFLink_CloseWebLinks(IntPtr linkPage);
    [DllImport(PdfiumLib)]
    public static extern int FPDFLink_CountRects(IntPtr linkPage, int linkIndex);
    [DllImport(PdfiumLib)]
    public static extern int FPDFLink_CountWebLinks(IntPtr linkPage);
    [DllImport(PdfiumLib)]
    public static extern bool FPDFLink_GetRect(IntPtr linkPage, int linkIndex, int rectIndex, out double left, out double top, out double right, out double bottom);
    [DllImport(PdfiumLib)]
    public static extern bool FPDFLink_GetTextRange(IntPtr linkPage, int linkIndex, out int startCharIndex, out int charCount);
    [DllImport(PdfiumLib)]
    public static extern int FPDFLink_GetURL(IntPtr linkPage, int linkIndex, IntPtr buffer, int buflen);
    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDFLink_LoadWebLinks(IntPtr textPage);
    [DllImport(PdfiumLib)]
    public static extern void FPDFText_ClosePage(IntPtr text_page);
    [DllImport(PdfiumLib)]
    public static extern int FPDFText_CountChars(IntPtr text_page);
    [DllImport(PdfiumLib)]
    public static extern int FPDFText_CountRects(IntPtr text_page, int start_index, int count);
    [DllImport(PdfiumLib)]
    public static extern void FPDFText_FindClose(IntPtr searchHandle);
    [DllImport(PdfiumLib)]
    public static extern bool FPDFText_FindNext(IntPtr searchHandle);
    [DllImport(PdfiumLib)]
    public static extern bool FPDFText_FindPrev(IntPtr searchHandle);
    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDFText_FindStart(IntPtr text_page, [MarshalAs(UnmanagedType.LPWStr)] string findwhat, uint flags, int start_index);
    [DllImport(PdfiumLib)]
    public static extern int FPDFText_GetBoundedText(IntPtr text_page, double left, double top, double right, double bottom, [Out] ushort[] buffer, int buflen);
    [DllImport(PdfiumLib)]
    public static extern float FPDFText_GetCharAngle(IntPtr textPage, int index);
    [DllImport(PdfiumLib)]
    public static extern bool FPDFText_GetCharBox(IntPtr text_page, int index, out double left, out double right, out double bottom, out double top);
    [DllImport(PdfiumLib)]
    public static extern int FPDFText_GetCharIndexAtPos(IntPtr text_page, double x, double y, double xTolerance, double yTolerance);
    [DllImport(PdfiumLib)]
    public static extern bool FPDFText_GetCharOrigin(IntPtr text_page, int index, out double x, out double y);
    [DllImport(PdfiumLib)]
    public static extern bool FPDFText_GetFillColor(IntPtr textPage, int index, out uint r, out uint g, out uint b, out uint a);
    [DllImport(PdfiumLib)]
    public static extern uint FPDFText_GetFontInfo(IntPtr textPage, int index, byte[] buffer, uint buflen, out int flags);
    [DllImport(PdfiumLib)]
    public static extern double FPDFText_GetFontSize(IntPtr textPage, int index);
    [DllImport(PdfiumLib)]
    public static extern int FPDFText_GetFontWeight(IntPtr textPage, int index);
    [DllImport(PdfiumLib)]
    public static extern bool FPDFText_GetLooseCharBox(IntPtr textPage, int index, out FsRectF rect);
    [DllImport(PdfiumLib)]
    public static extern bool FPDFText_GetMatrix(IntPtr textPage, int index, out FsMatrix matrix);
    [DllImport(PdfiumLib)]
    public static extern bool FPDFText_GetRect(IntPtr text_page, int rect_index, out double left, out double top, out double right, out double bottom);
    [DllImport(PdfiumLib)]
    public static extern int FPDFText_GetSchCount(IntPtr searchHandle);
    [DllImport(PdfiumLib)]
    public static extern int FPDFText_GetSchResultIndex(IntPtr searchHandle);
    [DllImport(PdfiumLib)]
    public static extern bool FPDFText_GetStrokeColor(IntPtr textPage, int index, out uint r, out uint g, out uint b, out uint a);
    [DllImport(PdfiumLib)]
    public static extern int FPDFText_GetText(IntPtr text_page, int start_index, int count, [Out] ushort[] result);
    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDFText_GetTextObject(IntPtr textPage, int index);
    [DllImport(PdfiumLib)]
    public static extern uint FPDFText_GetUnicode(IntPtr text_page, int index);
    [DllImport(PdfiumLib)]
    public static extern int FPDFText_HasUnicodeMapError(IntPtr textPage, int index);
    [DllImport(PdfiumLib)]
    public static extern int FPDFText_IsGenerated(IntPtr textPage, int index);
    [DllImport(PdfiumLib)]
    public static extern int FPDFText_IsHyphen(IntPtr textPage, int index);
    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDFText_LoadPage(IntPtr page);
}