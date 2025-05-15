using Nebulae.dotPDFium.Native;
using System;
using System.Runtime.InteropServices;

namespace nebulae.dotPDFium.Native;

public static class PdfSaveNative
{
    private const string PdfiumLib = "pdfium";

    [DllImport(PdfiumLib)]
    [return: MarshalAs(UnmanagedType.I1)]
    public static extern bool FPDF_SaveAsCopy(IntPtr document, ref FPDF_FILEWRITE writer, uint flags);

    [DllImport(PdfiumLib)]
    [return: MarshalAs(UnmanagedType.I1)]
    public static extern bool FPDF_SaveWithVersion(IntPtr document, ref FPDF_FILEWRITE writer, uint flags, int fileVersion);

    // Save flags
    public const int FPDF_INCREMENTAL = 1;
    public const int FPDF_NO_INCREMENTAL = 2;
    public const int FPDF_REMOVE_SECURITY = 3;
}

