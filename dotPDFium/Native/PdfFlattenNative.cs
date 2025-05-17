using System;
using System.Runtime.InteropServices;

namespace nebulae.dotPDFium.Native;

public static class PdfFlattenNative
{
    private const string PdfiumLib = "pdfium";

    // Flatten annotations and form fields into the page content stream
    [DllImport(PdfiumLib)]
    public static extern int FPDFPage_Flatten(IntPtr page, int nFlag);

    // Flatten result constants
    public const int FLATTEN_FAIL = 0;
    public const int FLATTEN_SUCCESS = 1;
    public const int FLATTEN_NOTHINGTODO = 2;

    // Flatten modes
    public const int FLAT_NORMALDISPLAY = 0;
    public const int FLAT_PRINT = 1;
}
