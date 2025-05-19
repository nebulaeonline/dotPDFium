using System;
using System.Runtime.InteropServices;

namespace nebulae.dotPDFium.Native;

public static class PdfDataAvailNative
{
    private const string PdfiumLib = "pdfium";

    // Create a document availability context from file access and availability interface
    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDFAvail_Create(ref FxFileAvail fileAvail, ref PdfFileAccess fileAccess);

    [DllImport(PdfiumLib)]
    public static extern void FPDFAvail_Destroy(IntPtr avail);

    [DllImport(PdfiumLib)]
    public static extern int FPDFAvail_IsDocAvail(IntPtr avail, ref FxDownloadHints hints);

    [DllImport(PdfiumLib)]
    public static extern IntPtr FPDFAvail_GetDocument(IntPtr avail, [MarshalAs(UnmanagedType.LPStr)] string password);

    [DllImport(PdfiumLib)]
    public static extern int FPDFAvail_IsPageAvail(IntPtr avail, int pageIndex, ref FxDownloadHints hints);

    [DllImport(PdfiumLib)]
    public static extern int FPDFAvail_IsFormAvail(IntPtr avail, ref FxDownloadHints hints);

    [DllImport(PdfiumLib)]
    public static extern int FPDFAvail_IsLinearized(IntPtr avail);

    [DllImport(PdfiumLib)]
    public static extern int FPDFAvail_GetFirstPageNum(IntPtr document);
}

