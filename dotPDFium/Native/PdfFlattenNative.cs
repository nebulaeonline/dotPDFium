using System;
using System.Runtime.InteropServices;

namespace nebulae.dotPDFium.Native;

public static class PdfFlattenNative
{
    private const string PdfiumLib = "pdfium";

    // Flatten annotations and form fields into the page content stream
    [DllImport(PdfiumLib)]
    public static extern int FPDFPage_Flatten(IntPtr page, int nFlag);
}
