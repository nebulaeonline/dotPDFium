using System;
using System.Runtime.InteropServices;

namespace nebulae.dotPDFium.Native;

public static class PdfDocNative
{
    private const string PdfiumLib = "pdfium";

    // --- Bookmarks ---
    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDFBookmark_GetFirstChild(IntPtr doc, IntPtr bookmark);

    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDFBookmark_GetNextSibling(IntPtr doc, IntPtr bookmark);

    [DllImport(PdfiumLib)]
    public static extern uint FPDFBookmark_GetTitle(IntPtr bookmark, [Out] char[] buffer, uint buflen);

    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDFBookmark_GetDest(IntPtr doc, IntPtr bookmark);

    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDFBookmark_GetAction(IntPtr bookmark);

    [DllImport(PdfiumLib)]
    public static extern int FPDFBookmark_GetCount(IntPtr bookmark);

    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDFBookmark_Find(IntPtr doc, [MarshalAs(UnmanagedType.LPWStr)] string title);

    // --- Actions & Destinations ---
    [DllImport(PdfiumLib)]
    public static extern uint FPDFAction_GetType(IntPtr action);

    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDFAction_GetDest(IntPtr doc, IntPtr action);

    [DllImport(PdfiumLib)]
    public static extern uint FPDFAction_GetFilePath(IntPtr action, [Out] byte[] buffer, uint buflen);

    [DllImport(PdfiumLib)]
    public static extern uint FPDFAction_GetURIPath(IntPtr doc, IntPtr action, [Out] byte[] buffer, uint buflen);

    [DllImport(PdfiumLib)]
    public static extern int FPDFDest_GetDestPageIndex(IntPtr doc, IntPtr dest);

    // --- Metadata ---
    [DllImport(PdfiumLib)]
    public static extern uint FPDF_GetMetaText(IntPtr doc, [MarshalAs(UnmanagedType.LPStr)] string tag, [Out] char[] buffer, uint buflen);

    [DllImport(PdfiumLib)]
    public static extern uint FPDF_GetPageLabel(IntPtr doc, int pageIndex, [Out] char[] buffer, uint buflen);

    // --- Links ---
    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDFLink_GetLinkAtPoint(IntPtr page, double x, double y);

    [DllImport(PdfiumLib)]
    public static extern int FPDFLink_GetLinkZOrderAtPoint(IntPtr page, double x, double y);

    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDFLink_GetDest(IntPtr doc, IntPtr link);

    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDFLink_GetAction(IntPtr link);
}
