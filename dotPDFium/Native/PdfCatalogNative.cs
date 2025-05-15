using System;
using System.Runtime.InteropServices;

namespace nebulae.dotPDFium.Native;

public static class PdfCatalogNative
{
    private const string PdfiumLib = "pdfium";

    // Check if the document is a tagged PDF
    [DllImport(PdfiumLib)]
    [return: MarshalAs(UnmanagedType.I1)]
    public static extern bool FPDFCatalog_IsTagged(IntPtr document);

    // Set the document language metadata (e.g. "en-US")
    [DllImport(PdfiumLib)]
    [return: MarshalAs(UnmanagedType.I1)]
    public static extern bool FPDFCatalog_SetLanguage(IntPtr document, [MarshalAs(UnmanagedType.LPStr)] string language);
}

